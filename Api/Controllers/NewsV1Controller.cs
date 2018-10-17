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
using Microsoft.AspNetCore.SignalR;
using Api.Hubs;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/news/")]
    [EnableCors("Default")]
    public class NewsV1Controller : NewsBaseController
    {
        public NewsV1Controller(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, IHubContext<AuctusHub> hubContext) :
            base(loggerFactory, cache, serviceProvider, serviceScopeFactory, hubContext) { }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ListNews([FromQuery]int? top, [FromQuery]int? lastNewsId)
        {
            return base.ListNews(top, lastNewsId);
        }

        [Route("news/testSignalR")]
        [HttpGet]
        public IActionResult Test()
        {
            HubContext.Clients.All.SendAsync("addLastNews",
                new Auctus.DomainObjects.News.News()
                {
                    CreationDate = DateTime.UtcNow,
                    ExternalCreationDate = DateTime.UtcNow,
                    ExternalId = "test",
                    Id = 1000,
                    Link = "https://auctus.org",
                    NewsSource = new Auctus.DomainObjects.News.NewsSource()
                    {
                        Id = 1,
                        Name = "Auctus"
                    },
                    Title = "Signal R Test"
                }
                );
            return Ok();
        }
    }
}
