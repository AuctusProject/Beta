using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Hubs;
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
        protected readonly IHubContext<AuctusHub> HubContext;
        protected JobBaseController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, IHubContext<AuctusHub> hubContext) : 
            base(loggerFactory, cache, serviceProvider, serviceScopeFactory)  {
            HubContext = hubContext;
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
                HubContext.Clients.All.SendAsync("addLastNews", news);
            });
            return Ok();
        }

        protected virtual IActionResult UpdateAssetsValues(string api)
        {
            RunAsync(() => AssetValueBusiness.UpdateCoingeckoAssetsValues());
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
                if (!string.IsNullOrWhiteSpace(api) && api.ToLower() == "coingecko")
                    base.OnActionExecuting(context);
                else
                    context.Result = new BadRequestResult();
            }
        }
    }
}