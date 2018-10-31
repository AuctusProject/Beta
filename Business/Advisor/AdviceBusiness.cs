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

        public Advice GetLastAdviceForAssetByAdvisor(int advisorId, int assetId)
        {
            return Data.GetLastAdviceForAssetByAdvisor(assetId, advisorId);
        }

        internal void ValidateAndCreate(DomainObjects.Advisor.Advisor advisor, DomainObjects.Asset.Asset asset, AdviceType type, double? stopLoss, double? targetPrice, double? price)
        {
            Advice lastAdvice = GetLastAdviceForAssetByAdvisor(advisor.Id, asset.Id);

            if (lastAdvice != null && Data.GetDateTimeNow().Subtract(lastAdvice.CreationDate).TotalSeconds < MinimumTimeInSecondsBetweenAdvices)
                throw new BusinessException(string.Format("You need to wait {0} seconds before emit a new signal for this asset.", MinimumTimeInSecondsBetweenAdvices - Math.Round(Data.GetDateTimeNow().Subtract(lastAdvice.CreationDate).TotalSeconds)));

            if (type == AdviceType.ClosePosition && (lastAdvice == null || lastAdvice.AdviceType == AdviceType.ClosePosition))
                throw new BusinessException("You need to set a Buy or Sell signal before emit a Close signal.");

            if (type == AdviceType.Sell && !asset.ShortSellingEnabled)
                throw new BusinessException("Sell signals are not available for this asset.");

            if (type == AdviceType.ClosePosition && (stopLoss.HasValue || targetPrice.HasValue))
                throw new BusinessException("Stop loss or take profit cannot be defined to a Close signal.");

            double? currentValue = null;
            if (!price.HasValue)
                currentValue = AssetCurrentValueBusiness.GetCurrentValue(asset.Id);
            else
                currentValue = AssetCurrentValueBusiness.GetRealCurrentValue(asset.Id);

            if (!currentValue.HasValue)
                throw new InvalidOperationException($"Asset {asset.Name} ({asset.Id}) does not have value defined.");

            if (stopLoss.HasValue && ((type == AdviceType.Buy && currentValue.Value <= stopLoss.Value) || (type == AdviceType.Sell && currentValue.Value >= stopLoss.Value)))
                throw new BusinessException($"Invalid stop loss value for current price {Util.Util.GetFormattedValue(currentValue.Value)}.");

            if (targetPrice.HasValue && ((type == AdviceType.Buy && currentValue.Value >= targetPrice.Value) || (type == AdviceType.Sell && currentValue.Value <= targetPrice.Value)))
                throw new BusinessException($"Invalid take profit value for current price {Util.Util.GetFormattedValue(currentValue.Value)}.");

            Insert(new Advice()
                    {
                        AdvisorId = advisor.Id,
                        AssetId = asset.Id,
                        Type = type.Value,
                        CreationDate = Data.GetDateTimeNow(),
                        AssetValue = price ?? currentValue.Value,
                        OperationType = AdviceOperationType.Manual.Value,
                        StopLoss = stopLoss,
                        TargetPrice = targetPrice
                    });

            RunAsync(() => SendAdviceNotificationForFollowersAsync(advisor, asset, type));
        }

        private void SendAdviceNotificationForFollowersAsync(DomainObjects.Advisor.Advisor advisor, DomainObjects.Asset.Asset asset, AdviceType type)
        {
            var usersFollowing = UserBusiness.ListValidUsersFollowingAdvisorOrAsset(advisor.Id, asset.Id);
            foreach (var user in usersFollowing)
                SendAdviceNotificationAsync(user, advisor, asset, type).Wait();
        }

        private async Task SendAdviceNotificationAsync(User user, DomainObjects.Advisor.Advisor advisor, DomainObjects.Asset.Asset asset, AdviceType type)
        {
            var imgUrl = Configuration.GetSection("Url:ProfileImgCdn").Get<string>().Replace("{id}", advisor.UrlGuid.ToString());
            var adviceTypeColorCode = "";
            switch (type.Value)
            {
                case 0:
                    adviceTypeColorCode = "#d13e3e";
                    break;
                case 1:
                    adviceTypeColorCode = "#3fd350";
                    break;
                default:
                    adviceTypeColorCode = "#333333";
                    break;
            }
            await EmailBusiness.SendUsingTemplateAsync(new string[] { user.Email },
                $"New signal on Auctus Experts for {asset.Code}",
                $@"
            <p>Expert {advisor.Name} set a new signal.</p>
			<a style=""text-decoration: none;color:#ffffff;"" href=""{WebUrl}/asset-details/{asset.Id}"">
				<div style=""border: solid 1px #404040;text-align:center;padding: 25px;"">
					<div style=""height:100px;width:100px;border-radius:50px;margin-right:27px;display:inline-block;vertical-align: top;"">
						<img style=""height:100px;width:100px;border-radius:50px;"" src=""{imgUrl}"">
					</div>
					<div style=""display:inline-block;"">
						<div style=""font-family: sans-serif; font-size: 19px;display:inline;"">
							<p style=""display:inline;""><strong>{asset.Code}</strong></p>
							<p style=""color:#cbcaca;display:inline;"">&nbsp;({asset.Name})</p>
						</div>
						<br />
						<div style=""width: 168px;background-color: {adviceTypeColorCode};color:#ffffff;font-size: 18px;text-align: center;padding: 14px;display:inline-block;margin-top:8px;"">{type.GetDescription()}</div>
					</div>
				</div>
			</a>
			<p style=""font-size:12px;font-style:italic;"">If you do not want to receive these signals for experts/assets that you are following, <a href='{WebUrl}?configuration=true' target='_blank'>click here</a>.</p>", 
                EmailTemplate.NotificationType.NewRecommendation);
        }

        public List<Advice> List(IEnumerable<int> advisorsId = null, IEnumerable<int> assetsId = null)
        {
            return Data.List(advisorsId, assetsId);
        }

        public List<Advice> ListAllCached()
        {
            var advicesCahceKey = "allAdvicesCached";
            var advices = MemoryCache.Get<List<Advice>>(advicesCahceKey);
            if (advices == null)
            {
                var advisors = AdvisorBusiness.GetAdvisors();
                if (advisors?.Any() == true)
                {
                    advices = List(advisors.Select(c => c.Id).Distinct());
                    if (advices.Any())
                        MemoryCache.Set<List<Advice>>(advicesCahceKey, advices, 20);
                }
                else
                    advices = new List<Advice>();
            }
            return advices;
        }

        public IEnumerable<Advice> ListLastAdvicesForUserWithPagination(IEnumerable<int> assetsId, int? top, int? lastAdviceId)
        {
            var followingAdvisorsIds = AdvisorBusiness.ListFollowingAdvisors().Select(c => c.Id).Distinct();
            return Data.ListLastAdvicesWithPagination(followingAdvisorsIds, assetsId, top, lastAdviceId);
        }

        public IEnumerable<FeedResponse> ListLastAdvicesForAllTypes(int? numberOfAdvicesOfEachType)
        {
            var advicesForFeed = Task.Factory.StartNew(() => Data.ListLastAdvicesForAllTypes(numberOfAdvicesOfEachType));
            return UserBusiness.FillFeedList(advicesForFeed, null, null, null, null, null, null, null);
        }

        public IEnumerable<int> ListTrendingAdvisedAssets(int top = 3)
        {
            return Data.ListTrendingAdvisedAssets(top);
        }

        public void InsertAutomatedClosePositionAdvices(List<Advice> automatedAdvices)
        {
            Data.InsertAdvices(automatedAdvices);
        }
    }
}
