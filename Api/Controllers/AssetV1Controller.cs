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

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/assets/")]
    [EnableCors("Default")]
    public class AssetV1Controller : AssetBaseController
    {
        public AssetV1Controller(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider) : base(loggerFactory, cache, serviceProvider)
        {
        }

        [HttpGet]
        //[Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ListAssets()
        {
            return base.ListAssets();
        }

        [Route("{id}/followers")]
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult FollowAsset([FromRoute]int id)
        {
            return base.FollowAsset(id);
        }

        [Route("{id}/followers")]
        [HttpDelete]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult UnfollowAsset([FromRoute]int id)
        {
            return base.UnfollowAsset(id);
        }

        
       

    }
}
