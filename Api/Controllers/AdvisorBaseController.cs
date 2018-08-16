using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Model.Advisor;
using Auctus.DomainObjects.Advisor;
using Auctus.Model;
using Auctus.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Auctus.DomainObjects.Account;

namespace Api.Controllers
{
    public class AdvisorBaseController : BaseController
    {
        protected AdvisorBaseController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider) : base(loggerFactory, cache, serviceProvider) { }

        protected IActionResult GetAdvisor(int id)
        {
            return Ok(AdvisorBusiness.GetAdvisorData(id));
        }

        protected IActionResult ListAdvisors()
        {
            return Ok(AdvisorBusiness.ListAdvisorsData());
        }

        protected IActionResult Advise(AdviseRequest adviseRequest)
        {
            if (adviseRequest == null || adviseRequest.AssetId == 0 || AdviceType.Get(adviseRequest.AdviceType)==null)
                return BadRequest();

            AdvisorBusiness.Advise(adviseRequest.AssetId, AdviceType.Get(adviseRequest.AdviceType));
            return Ok();
        }

        protected async Task<IActionResult> RequestToBe(BeAdvisorRequest beAdvisorRequest)
        {
            if (beAdvisorRequest == null)
                return BadRequest();

            return Ok(await RequestToBeAdvisorBusiness.Create(beAdvisorRequest.Name, beAdvisorRequest.Description, beAdvisorRequest.PreviousExperience));
        }

        protected IActionResult GetRequestToBe()
        {
            return Ok(RequestToBeAdvisorBusiness.GetByLoggedEmail());
        }

        protected virtual IActionResult FollowAdvisor(int id)
        {
            return Ok(UserBusiness.FollowUnfollowAdvisor(id, FollowActionType.Follow));
        }

        protected virtual IActionResult UnfollowAdvisor(int id)
        {
            return Ok(UserBusiness.FollowUnfollowAdvisor(id, FollowActionType.Unfollow));
        }
    }
}