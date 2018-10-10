using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Auctus.DomainObjects.Account;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Controllers
{
    public class AssetBaseController : BaseController
    {
        protected AssetBaseController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory) :
            base(loggerFactory, cache, serviceProvider, serviceScopeFactory) { }

        protected IActionResult ListAssets()
        {
            var assetResponse = AssetBusiness.ListAssetsOrderedByMarketCap();
            return Ok(assetResponse);            
        }

        protected IActionResult ListTrendingAssets(int top = 3)
        {
            var assetResponse = AssetBusiness.ListTrendingAssets(top);
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

        protected IActionResult GetAsset(int id)
        {
            return Ok(AssetBusiness.GetAssetData(id));
        }

        protected IActionResult GetAssetRecommendationInfo(int id)
        {
            return Ok(AssetBusiness.GetAssetRecommendationInfo(id));
        }
        
        protected IActionResult FollowAsset(int id)
        {
            return Ok(UserBusiness.FollowUnfollowAsset(id, FollowActionType.Follow));
        }

        protected IActionResult UnfollowAsset(int id)
        {
            return Ok(UserBusiness.FollowUnfollowAsset(id, FollowActionType.Unfollow));
        }
    }
}
