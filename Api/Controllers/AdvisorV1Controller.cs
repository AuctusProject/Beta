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
    [EnableCors("Default")]
    public class AdvisorV1Controller : AdvisorBaseController
    {
        public AdvisorV1Controller(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory) :
            base(loggerFactory, cache, serviceProvider, serviceScopeFactory) { }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ListAdvisors()
        {
            return base.ListAdvisors();
        }

        [Route("{id}")]
        [HttpGet]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult GetAdvisor(int id)
        {
            return base.GetAdvisor(id);
        }

        [Route("{id}/details")]
        [HttpGet]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult GetAdvisorDetails(int id)
        {
            return base.GetAdvisorDetails(id);
        }

        [Route("{id}")]
        [HttpPost]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new async Task<IActionResult> EditAdvisorAsync(int id, [FromForm]AdvisorRequest advisorRequest)
        {
            return await base.EditAdvisorAsync(id, advisorRequest);
        }

        [Route("advices")]
        [HttpPost]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult Advise([FromBody]AdviseRequest adviseRequest)
        {
            return base.Advise(adviseRequest);
        }

        [Route("me/requests")]
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new async Task<IActionResult> RequestToBeAsync([FromForm]BeAdvisorRequest beAdvisorRequest)
        {
            return await base.RequestToBeAsync(beAdvisorRequest);
        }

        [Route("me/requests")]
        [HttpGet]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult GetRequestToBe()
        {
            return base.GetRequestToBe();
        }

        [Route("requests")]
        [HttpGet]
        [OnlyAdmin]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ListRequestToBe()
        {
            return base.ListRequestToBe();
        }

        [Route("requests/{id}/approve")]
        [HttpPost]
        [OnlyAdmin]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new async Task<IActionResult> ApproveRequestToBeAsync(int id)
        {
            return await base.ApproveRequestToBeAsync(id);
        }

        [Route("requests/{id}/reject")]
        [HttpPost]
        [OnlyAdmin]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new async Task<IActionResult> RejectRequestToBeAsync(int id)
        {
            return await base.RejectRequestToBeAsync(id);
        }

        [Route("{id}/followers")]
        [HttpPost]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult FollowAdvisor([FromRoute]int id)
        {
            return base.FollowAdvisor(id);
        }

        [Route("{id}/followers")]
        [HttpDelete]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult UnfollowAdvisor([FromRoute]int id)
        {
            return base.UnfollowAdvisor(id);
        }

        [Route("advices/latest_by_type")]
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ListLastAdvicesForAllTypes([FromQuery]int numberOfAdvicesOfEachType)
        {
            return base.ListLastAdvicesForAllTypes(numberOfAdvicesOfEachType);
        }
    }
}