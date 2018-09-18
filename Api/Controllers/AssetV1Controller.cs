using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/assets/")]
    [EnableCors("Default")]
    public class AssetV1Controller : AssetBaseController
    {
        public AssetV1Controller(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory) :
            base(loggerFactory, cache, serviceProvider, serviceScopeFactory) { }

        [HttpGet]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ListAssets()
        {
            return base.ListAssets();
        }

        [HttpGet]
        [Route("trending/{top?}")]
        /* [Authorize("Bearer")]*/
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ListTrendingAssets(int top = 3)
        {
            return base.ListTrendingAssets(top);
        }

        [HttpGet]
        [Route("details")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ListAssetsDetails()
        {
            return base.ListAssetsDetails();
        }

        [HttpGet]
        [Route("{id}/details")]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult GetAsset(int id)
        {
            return base.GetAsset(id);
        }

        [HttpGet]
        [Route("{id}/recommendation_info")]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult GetAssetRecommendationInfo(int id)
        {
            return base.GetAssetRecommendationInfo(id);
        }

        [Route("{id}/followers")]
        [HttpPost]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult FollowAsset([FromRoute]int id)
        {
            return base.FollowAsset(id);
        }

        [Route("{id}/followers")]
        [HttpDelete]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult UnfollowAsset([FromRoute]int id)
        {
            return base.UnfollowAsset(id);
        }
    }
}
