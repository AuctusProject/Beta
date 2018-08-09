using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auctus.Model;
using Auctus.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Cors;
using Api.Model.Advisor;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/advisors/")]
    [EnableCors("Default")]
    public class AdvisorV1Controller : AdvisorBaseController
    {
        public AdvisorV1Controller(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider) : base(loggerFactory, cache, serviceProvider) { }

        [Route("advices")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult Advise([FromBody]AdviseRequest adviseRequest)
        {
            return base.Advise(adviseRequest);
        }

        [Route("me/requests")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new async Task<IActionResult> RequestToBe([FromBody]BeAdvisorRequest beAdvisorRequest)
        {
            return await base.RequestToBe(beAdvisorRequest);
        }

        [Route("me/requests")]
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult GetRequestToBe()
        {
            return base.GetRequestToBe();
        }
    }
}