using Api.Model.Account;
using Auctus.DomainObjects.Account;
using Auctus.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/jobs/")]
    [Authorize("Bearer")]
    [EnableCors("Default")]
    public class JobV1Controller : JobBaseController
    {
        public JobV1Controller(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory) : base(loggerFactory, cache, serviceProvider, serviceScopeFactory) { }

        [Route("assets/{api}/values")]
        [HttpPost]
        public new IActionResult UpdateAssetsValues([FromRoute]string api)
        {
            return base.UpdateAssetsValues(api);
        }

        [Route("assets/{api}")]
        [HttpPost]
        public new IActionResult CreateAssets([FromRoute]string api)
        {
            return base.CreateAssets(api);
        }

        [Route("assets/{api}/icons")]
        [HttpPost]
        public new IActionResult UpdateAssetsIcons([FromRoute]string api)
        {
            return base.UpdateAssetsIcons(api);
        }

        [Route("assets/{api}/marketcap")]
        [HttpPost]
        public new IActionResult UpdateAssetsMarketcap([FromRoute]string api)
        {
            return base.UpdateAssetsMarketcap(api);
        }

        [Route("users/auc")]
        [HttpPost]
        public new IActionResult SetUsersAuc()
        {
            return base.SetUsersAuc();
        }
    }
}
