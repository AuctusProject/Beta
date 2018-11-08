using Api.Model.Account;
using Auctus.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/website/")]
    [EnableCors("AllowAll")]
    public class WebsiteExternalV1Controller : WebsiteExternalBaseController
    {
        public WebsiteExternalV1Controller(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory) :
            base(loggerFactory, cache, serviceProvider, serviceScopeFactory) { }

        [Route("emails")]
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new async Task<IActionResult> IncludeSubscribedEmailFromWebsite([FromBody]EarlyAccessRequest emailRequest)
        {
            return await base.IncludeSubscribedEmailFromWebsite(emailRequest);
        }
    }
}
