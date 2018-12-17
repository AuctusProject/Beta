using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Auctus.DomainObjects.Account;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using Api.Hubs;

namespace Api.Controllers
{
    public class AssetBaseController : BaseController
    {
        protected AssetBaseController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, IHubContext<AuctusHub> hubContext) :
            base(loggerFactory, cache, serviceProvider, serviceScopeFactory, hubContext) { }

        protected IActionResult ListAssets()
        {
            var assetResponse = AssetBusiness.ListAssetsOrderedByMarketCap();
            return Ok(assetResponse);            
        }

        protected IActionResult ListReports(int? top, int? lastReportId, int? assetId)
        {
            return Ok(ReportBusiness.ListReports(top, lastReportId, assetId));
        }

        protected IActionResult ListEvents(int? top, int? lastEventId, int? assetId)
        {
            return Ok(AssetEventBusiness.ListAssetEvents(top, lastEventId, assetId));
        }

        protected IActionResult ListAssetValues(int id, DateTime? dateTime)
        {
            return Ok(AssetValueBusiness.ListAssetValues(id, dateTime));
        }

        protected IActionResult ListAssetsDetails()
        {
            var assetResponse = AssetBusiness.ListAssetData();
            return Ok(assetResponse);
        }

        protected IActionResult ListAssetsForTerminal()
        {
            return Ok(AssetBusiness.ListAssetsForTerminal());
        }

        protected IActionResult ListAssetBaseData(int id)
        {
            return Ok(AssetBusiness.ListAssetBaseData(id));
        }

        protected IActionResult ListAssetStatus(int id)
        {
            return Ok(AssetBusiness.ListAssetStatus(id));
        }

        protected IActionResult ListAssetOrders(int id)
        {
            return Ok(OrderBusiness.ListLastAdvisorsOrdersForAsset(id));
        }

        protected IActionResult GetAsset(int id)
        {
            return Ok(AssetBusiness.GetAssetData(id));
        }

        protected IActionResult FollowAsset(int id)
        {
            return Ok(UserBusiness.FollowUnfollowAsset(id, FollowActionType.Follow));
        }

        protected IActionResult UnfollowAsset(int id)
        {
            return Ok(UserBusiness.FollowUnfollowAsset(id, FollowActionType.Unfollow));
        }

        protected IActionResult ListTrendingAssets(int? listSize)
        {
            var assetResponse = AssetBusiness.ListTrendingAssets(listSize);
            return Ok(assetResponse);
        }
    }
}
