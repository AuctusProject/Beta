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
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace Api.Controllers
{
    public class AdvisorBaseController : BaseController
    {
        protected AdvisorBaseController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory) :
            base(loggerFactory, cache, serviceProvider, serviceScopeFactory) { }

        protected IActionResult GetAdvisor(int id)
        {
            return Ok(AdvisorBusiness.GetAdvisor(id));
        }

        protected IActionResult GetAdvisorDetails(int id)
        {
            return Ok(AdvisorBusiness.GetAdvisorData(id));
        }

        protected async Task<IActionResult> EditAdvisorAsync(int id, AdvisorRequest advisorRequest)
        {
            if (advisorRequest.ChangedPicture && RequestHasFile())
            {
                var fileExtension = GetValidPictureExtension();
                using (var stream = Request.Form.Files[0].OpenReadStream())
                    return Ok(await AdvisorBusiness.EditAdvisorAsync(id, advisorRequest.Name, advisorRequest.Description, true, stream, fileExtension));
            }
            else
                return Ok(await AdvisorBusiness.EditAdvisorAsync(id, advisorRequest.Name, advisorRequest.Description, advisorRequest.ChangedPicture, null, null));
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

        protected async Task<IActionResult> RequestToBeAsync(BeAdvisorRequest beAdvisorRequest)
        {
            if (beAdvisorRequest == null)
                return BadRequest();

            if (GetUser() == null && !IsValidRecaptcha(beAdvisorRequest.Captcha))
                return BadRequest(new { error = "Invalid Captcha." });

            LoginResponse response;
            if (beAdvisorRequest.ChangedPicture && RequestHasFile())
            {
                var fileExtension = GetValidPictureExtension();
                using (var stream = Request.Form.Files[0].OpenReadStream())
                    response = await RequestToBeAdvisorBusiness.CreateAsync(beAdvisorRequest.Email, beAdvisorRequest.Password, beAdvisorRequest.Name, beAdvisorRequest.Description,
                        beAdvisorRequest.PreviousExperience, beAdvisorRequest.ChangedPicture, stream, fileExtension);
            }
            else
                response = await RequestToBeAdvisorBusiness.CreateAsync(beAdvisorRequest.Email, beAdvisorRequest.Password, beAdvisorRequest.Name, beAdvisorRequest.Description, 
                    beAdvisorRequest.PreviousExperience, beAdvisorRequest.ChangedPicture, null, null);

            return Ok(new { logged = !response.PendingConfirmation, jwt = GenerateToken(response.Email.ToLower().Trim()), data = response });
        }

        protected IActionResult GetRequestToBe()
        {
            return Ok(RequestToBeAdvisorBusiness.GetByLoggedEmail());
        }

        protected IActionResult ListRequestToBe()
        {
            return Ok(RequestToBeAdvisorBusiness.ListPending());
        }

        protected async Task<IActionResult> ApproveRequestToBeAsync(int id)
        {
            await RequestToBeAdvisorBusiness.ApproveAsync(id);
            return Ok();
        }

        protected async Task<IActionResult> RejectRequestToBeAsync(int id)
        {
            await RequestToBeAdvisorBusiness.RejectAsync(id);
            return Ok();
        }

        protected virtual IActionResult FollowAdvisor(int id)
        {
            return Ok(UserBusiness.FollowUnfollowAdvisor(id, FollowActionType.Follow));
        }

        protected virtual IActionResult UnfollowAdvisor(int id)
        {
            return Ok(UserBusiness.FollowUnfollowAdvisor(id, FollowActionType.Unfollow));
        }

        protected virtual IActionResult ListLastAdvicesForAllTypes(int numberOfAdvicesOfEachType)
        {
            return Ok(AdviceBusiness.ListLastAdvicesForAllTypes(numberOfAdvicesOfEachType));
        }
    }
}