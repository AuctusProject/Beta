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
using Microsoft.Extensions.DependencyInjection;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/advisors/")]
    [Authorize("Bearer")]
    [EnableCors("Default")]
    public class AdvisorV1Controller : AdvisorBaseController
    {
        public AdvisorV1Controller(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory) :
            base(loggerFactory, cache, serviceProvider, serviceScopeFactory) { }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ListAdvisors()
        {
            return base.ListAdvisors();
        }

        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult GetAdvisor(int id)
        {
            return base.GetAdvisor(id);
        }

        [Route("{id}/details")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult GetAdvisorDetails(int id)
        {
            return base.GetAdvisorDetails(id);
        }

        [Route("{id}")]
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult EditAdvisor(int id, [FromBody]AdvisorRequest advisorRequest)
        {
            return base.EditAdvisor(id, advisorRequest);
        }

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
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult GetRequestToBe()
        {
            return base.GetRequestToBe();
        }

        [Route("requests")]
        [HttpGet]
        [OnlyAdmin]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ListRequestToBe()
        {
            return base.ListRequestToBe();
        }

        [Route("requests/{id}/approve")]
        [HttpPost]
        [OnlyAdmin]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ApproveRequestToBe(int id)
        {
            return base.ApproveRequestToBe(id);
        }

        [Route("requests/{id}/reject")]
        [HttpPost]
        [OnlyAdmin]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult RejectRequestToBe(int id)
        {
            return base.RejectRequestToBe(id);
        }

        [Route("{id}/followers")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult FollowAdvisor([FromRoute]int id)
        {
            return base.FollowAdvisor(id);
        }

        [Route("{id}/followers")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult UnfollowAdvisor([FromRoute]int id)
        {
            return base.UnfollowAdvisor(id);
        }

        [Route("advices/latest_by_type")]
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ListLastAdvicesForAllTypes([FromQuery]int top)
        {
            return base.ListLastAdvicesForAllTypes(top);
        }
    }
}