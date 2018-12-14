using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Hubs;
using Auctus.DomainObjects.Trade;
using Auctus.Model;
using Auctus.Util;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    public class JobBaseController : BaseController
    {
        protected JobBaseController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, IHubContext<AuctusHub> hubContext) :
            base(loggerFactory, cache, serviceProvider, serviceScopeFactory, hubContext) { }

        protected virtual IActionResult UpdateAdvisorsRankingAndProfit()
        {
            RunAsync(() => AdvisorRankingBusiness.SetAdvisorRankingAndProfit());
            return Ok();
        }

        protected virtual IActionResult SetAdvisorsRankingAndProfitHistory()
        {
            RunAsync(() => AdvisorRankingHistoryBusiness.SetAdvisorRankingAndProfitHistory());
            return Ok();
        }

        protected virtual IActionResult SetAdvisorsMonthlyRanking()
        {
            RunAsync(() => AdvisorMonthlyRankingBusiness.SetAdvisorsMonthlyRanking());
            return Ok();
        }

        protected virtual IActionResult UpdateAssetsEvents()
        {
            RunAsync(() => AssetEventBusiness.UpdateAssetEventsAsync());
            return Ok();
        }

        protected virtual IActionResult UpdateLastNews()
        {
            RunAsync(() => {
                var news = NewsBusiness.UpdateLastNews();
                HubContext.Clients.All.SendAsync("addLastNews", news).Wait();
            });
            return Ok();
        }

        protected virtual IActionResult UpdateAssetsValues(string api)
        {
            RunAsync(() =>
            {
                Dictionary<int, Dictionary<OrderActionType, List<OrderResponse>>> result;
                if (api == "coingecko")
                    result = AssetValueBusiness.UpdateCoingeckoAssetsValues();
                else
                    result = AssetValueBusiness.UpdateBinanceAssetsValues();

                if (result != null && result.Any())
                {
                    foreach (var ordersType in result)
                    {
                        var advisor = AdvisorRankingBusiness.GetAdvisorFullData(ordersType.Key);
                        if (advisor != null)
                        {
                            foreach (var orders in ordersType.Value)
                            {
                                var methodName = orders.Key == OrderActionType.StopLoss ? "onReachStopLoss" :
                                    orders.Key == OrderActionType.TakeProfit ? "onReachTakeProfit" : "onReachOrderLimit";

                                HubContext.Clients.User(advisor.Email).SendAsync(methodName, orders.Value);
                            }
                            var followers = UserBusiness.GetUserFromCache(advisor.Email)?.FollowingUsers;
                            if (followers?.Any() == true)
                            {
                                var respectiveOrders = ordersType.Value.Values.SelectMany(c => c).ToList();
                                foreach (var user in followers)
                                    HubContext.Clients.User(user).SendAsync("onNewTradeSignal", respectiveOrders);
                            }
                        }
                    }
                }
            });
            return Ok();
        }

        protected virtual IActionResult UpdateAssetsValues7dAnd30d(string api)
        {
            RunAsync(() =>
            {
                AssetValueBusiness.UpdateBinanceAssetsValues7dAnd30d();
            });
            return Ok();
        }

        protected virtual IActionResult UpdateAssetsMarketcap(string api)
        {
            RunAsync(() => AssetBusiness.UpdateCoingeckoAssetsMarketcap());
            return Ok();
        }

        protected virtual IActionResult CreateAssets(string api)
        {
            RunAsync(async () => await AssetBusiness.CreateCoingeckoNotIncludedAssetsAsync());
            return Ok();
        }

        protected virtual IActionResult UpdateAssetsIcons(string api)
        {
            RunAsync(async () => await AssetBusiness.UpdateCoingeckoAssetsIconsAsync());
            return Ok();
        }

        protected virtual IActionResult SetUsersAuc()
        {
            RunAsync(() => UserBusiness.SetUsersAucSituation());
            return Ok();
        }

        [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
        protected class ValidApiAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext context)
            {
                var api = context.RouteData?.Values.Any() == true ? context.RouteData.Values["api"]?.ToString() : null;
                if (!string.IsNullOrWhiteSpace(api) && (api.ToLower() == "coingecko" || api.ToLower() == "binance"))
                    base.OnActionExecuting(context);
                else
                    context.Result = new BadRequestResult();
            }
        }
    }
}