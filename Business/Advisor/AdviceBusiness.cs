using Auctus.DataAccess.Advisor;
using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using Auctus.Model;
using Auctus.Util;
using Auctus.Util.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Auctus.Business.Advisor.AdvisorBusiness;

namespace Auctus.Business.Advisor
{
    public class AdviceBusiness : BaseBusiness<Advice, IAdviceData<Advice>>
    {
        public AdviceBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, loggerFactory, cache, email, ip) { }

        internal void ValidateAndCreate(int advisorId, DomainObjects.Asset.Asset asset, AdviceType type)
        {
            Advice lastAdvice = Data.GetLastAdviceForAssetByAdvisor(asset.Id, advisorId);

            if (lastAdvice != null && Data.GetDateTimeNow().Subtract(lastAdvice.CreationDate).TotalSeconds < MinimumTimeInSecondsBetweenAdvices)
                throw new BusinessException("You need to wait before advising again for this asset.");

            if (type == AdviceType.ClosePosition && (lastAdvice == null || lastAdvice.AdviceType == AdviceType.ClosePosition))
                throw new BusinessException("You need a Buy or Sell recommendation before advising to Close Position.");

            if (type == AdviceType.Sell && !asset.ShortSellingEnabled)
                throw new BusinessException("Sell recommendations are not available for this asset.");

            Insert(new Advice()
                    {
                        AdvisorId = advisorId,
                        AssetId = asset.Id,
                        Type = type.Value,
                        CreationDate = Data.GetDateTimeNow()
                    });

            var usersFollowing = UserBusiness.ListUsersFollowingAdvisorOrAsset(advisorId, asset.Id);
        }

        public List<Advice> List(IEnumerable<int> advisorsId)
        {
            return Data.List(advisorsId);
        }

        public IEnumerable<Advice> ListLastAdvicesForUserWithPagination(int? top, int? lastAdviceId)
        {
            var followingAdvisorsIds = Task.Factory.StartNew(() => AdvisorBusiness.ListFollowingAdvisors().Select(c => c.Id));
            var followingAssetsIds = Task.Factory.StartNew(() => AssetBusiness.ListFollowingAssets().Select(c => c.Id));
            Task.WaitAll(followingAdvisorsIds, followingAssetsIds);
            return Data.ListLastAdvicesWithPagination(followingAdvisorsIds.Result, followingAssetsIds.Result, top, lastAdviceId);
        }

        public IEnumerable<FeedResponse> ListFeed(int? top, int? lastAdviceId)
        {
            var advicesForFeed = Task.Factory.StartNew(() => ListLastAdvicesForUserWithPagination(top, lastAdviceId));

            string advisorsCacheKey = "FeedAdvisorsResult" + LoggedEmail;
            string assetsCacheKey = "FeedAssetsResult" + LoggedEmail;
            var advisorsResult = MemoryCache.Get<List<AdvisorResponse>>(advisorsCacheKey);
            var assetsResult = MemoryCache.Get<List<AssetResponse>>(assetsCacheKey);
            if (advisorsResult == null || assetsResult == null || !lastAdviceId.HasValue)
            {
                var user = GetValidUser();
                var advisors = AdvisorBusiness.GetAdvisors();
                var assets = AssetBusiness.ListAssets();
                var advices = Task.Factory.StartNew(() => AdviceBusiness.List(advisors.Select(c => c.Id).Distinct()));
                var advisorFollowers = Task.Factory.StartNew(() => FollowAdvisorBusiness.ListFollowers(advisors.Select(c => c.Id).Distinct()));
                var assetFollowers = Task.Factory.StartNew(() => FollowAssetBusiness.ListFollowers());
                Task.WaitAll(advices, advisorFollowers, assetFollowers);

                AdvisorBusiness.Calculation(CalculationMode.Feed, out advisorsResult, out assetsResult, user, advices.Result, advisors, advisorFollowers.Result, assetFollowers.Result);

                MemoryCache.Set(advisorsCacheKey, advisorsResult, 10);
                MemoryCache.Set(assetsCacheKey, assetsResult, 10);
            }

            Task.WaitAll(advicesForFeed);
            return ConvertToFeedResponse(advicesForFeed.Result, advisorsResult, assetsResult);
        }

        private static List<FeedResponse> ConvertToFeedResponse(IEnumerable<Advice> advices, List<AdvisorResponse> advisorsResult, List<AssetResponse> assetResult)
        {
            var feedResult = new List<FeedResponse>();

            foreach (var advice in advices)
            {
                var advisorResponse = advisorsResult.First(c => c.UserId == advice.AdvisorId);
                var assetResponse = assetResult.First(c => c.AssetId == advice.AssetId);
                feedResult.Add(new FeedResponse()
                {
                    AdviceId = advice.Id,
                    AdviceType = advice.AdviceType.Value,
                    AdvisorId = advice.AdvisorId,
                    AssetId = advice.AssetId,
                    Date = advice.CreationDate,
                    AdvisorName = advisorResponse.Name,
                    AdvisorRanking = advisorResponse.Ranking,
                    FollowingAdvisor = advisorResponse.Following,
                    AssetCode = assetResponse.Code,
                    AssetName = assetResponse.Name,
                    FollowingAsset = assetResponse.Following == true
                });
            }
            return feedResult.OrderByDescending(c => c.Date).ToList();
        }
    }
}
