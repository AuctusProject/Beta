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
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ListAssets()
        {
            return base.ListAssets();
        }

        [HttpGet]
        [Route("trending/{top?}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ListTrendingAssets(int top = 3)
        {
            return base.ListTrendingAssets(top);
        }

        [Route("reports")]
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ListReports([FromQuery]int? top, [FromQuery]int? lastReportId)
        {
            return base.ListReports(top, lastReportId);
        }

        [Route("events")]
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ListEvents([FromQuery]int? top, [FromQuery]int? lastEventId, [FromQuery]int? assetId)
        {
            return base.ListEvents(top, lastEventId, assetId);
        }

        [HttpGet]
        [Route("{id}/values")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ListAssetValues([FromRoute] int id, [FromQuery] DateTime? dateTime)
        {
            return base.ListAssetValues(id, dateTime);
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
        [AllowAnonymous]
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
