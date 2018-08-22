using Auctus.DataAccess.Advisor;
using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
using Auctus.Model;
using Auctus.Util;
using Auctus.Util.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        public AdviceBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        internal void ValidateAndCreate(DomainObjects.Advisor.Advisor advisor, DomainObjects.Asset.Asset asset, AdviceType type)
        {
            Advice lastAdvice = Data.GetLastAdviceForAssetByAdvisor(asset.Id, advisor.Id);

            if (lastAdvice != null && Data.GetDateTimeNow().Subtract(lastAdvice.CreationDate).TotalSeconds < MinimumTimeInSecondsBetweenAdvices)
                throw new BusinessException("You need to wait before advising again for this asset.");

            if (type == AdviceType.ClosePosition && (lastAdvice == null || lastAdvice.AdviceType == AdviceType.ClosePosition))
                throw new BusinessException("You need a Buy or Sell recommendation before advising to Close Position.");

            if (type == AdviceType.Sell && !asset.ShortSellingEnabled)
                throw new BusinessException("Sell recommendations are not available for this asset.");

            Insert(new Advice()
                    {
                        AdvisorId = advisor.Id,
                        AssetId = asset.Id,
                        Type = type.Value,
                        CreationDate = Data.GetDateTimeNow()
                    });

            RunAsync(async () => await SendAdviceNotificationForFollowersAsync(advisor, asset, type));
        }

        private async Task SendAdviceNotificationForFollowersAsync(DomainObjects.Advisor.Advisor advisor, DomainObjects.Asset.Asset asset, AdviceType type)
        {
            var usersFollowing = UserBusiness.ListUsersFollowingAdvisorOrAsset(advisor.Id, asset.Id);
            foreach (var user in usersFollowing)
                await SendAdviceNotification(user, advisor, asset, type);
        }

        private async Task SendAdviceNotification(User user, DomainObjects.Advisor.Advisor advisor, DomainObjects.Asset.Asset asset, AdviceType type)
        {
            await EmailBusiness.SendAsync(new string[] { user.Email },
                $"New tip on Auctus Beta for {asset.Code}",
                $@"Hello,
<br/><br/>
The advisor {advisor.Name} set a new {type.GetDescription()} tip for the asset {asset.Code} - {asset.Name}.
<br/>
To see more details <a href='{WebUrl}/asset-details/{asset.Id}' target='_blank'>click here</a>.
<br/><br/>
<small>If you do not want to receive these tips for advisors/assets that you are following <a href='{WebUrl}/configuration' target='_blank'>click here</a>.</small>
<br/><br/>
Thanks,
<br/>
Auctus Team");
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
