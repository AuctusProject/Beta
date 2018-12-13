using Api.Hubs;
using Api.Model.Account;
using Auctus.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    public class WebsiteExternalBaseController : BaseController
    {
        protected WebsiteExternalBaseController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, IHubContext<AuctusHub> hubContext) :
            base(loggerFactory, cache, serviceProvider, serviceScopeFactory, hubContext) { }

        protected virtual async Task<IActionResult> IncludeSubscribedEmailFromWebsite(EarlyAccessRequest emailRequest)
        {
            await EmailBusiness.IncludeSubscribedEmailFromWebsite(emailRequest.Email, emailRequest.Name);
            return Ok();
        }
    }
}
