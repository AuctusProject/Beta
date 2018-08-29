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
            var assetResponse = AssetBusiness.ListAssets();
            return Ok(assetResponse);
            
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
