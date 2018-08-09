using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Auctus.DomainObjects.Account;

namespace Api.Controllers
{
    public class AssetBaseController : BaseController
    {
        protected AssetBaseController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider) : base(loggerFactory, cache, serviceProvider)
        {
        }

        protected virtual IActionResult FollowAsset(int id)
        {
            try
            {
                var followAsset = UserBusiness.FollowUnfollowAsset(id, FollowActionType.Follow);
                return Ok(followAsset);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        protected virtual IActionResult UnfollowAsset(int id)
        {
            try
            {
                var followAsset = UserBusiness.FollowUnfollowAsset(id, FollowActionType.Unfollow);
                return Ok(followAsset);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
