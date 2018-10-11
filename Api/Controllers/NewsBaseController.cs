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
    public class NewsBaseController : BaseController
    {
        protected NewsBaseController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory) :
            base(loggerFactory, cache, serviceProvider, serviceScopeFactory) { }

        protected IActionResult ListNews()
        {
            NewsBusiness.CreateNews();
            return Ok();            
        }
    }
}
