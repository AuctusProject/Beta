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
            if (advisorRequest.ChangedPicture && Request.Form != null && Request.Form.Files != null && Request.Form.Files.Count == 1 && Request.Form.Files[0].Length > 0
                && !string.IsNullOrWhiteSpace(Request.Form.Files[0].FileName) && !string.IsNullOrWhiteSpace(Request.Form.Files[0].ContentType))
            {
                if (!FileTypeMatcher.GetValidFileExtensions().Any(c => Request.Form.Files[0].ContentType.ToUpper().Contains(c)))
                    return BadRequest(new { error = "Invalid file." });

                var fileExtension = Request.Form.Files[0].FileName.Split('.').Last().ToUpper();
                if (string.IsNullOrWhiteSpace(fileExtension) || !FileTypeMatcher.GetValidFileExtensions().Any(c => c == fileExtension))
                    return BadRequest(new { error = "File extension is invalid." });

                using (var stream = Request.Form.Files[0].OpenReadStream())
                    await AdvisorBusiness.EditAdvisorAsync(id, advisorRequest.Name, advisorRequest.Description, true, stream, fileExtension);
            }
            else
                await AdvisorBusiness.EditAdvisorAsync(id, advisorRequest.Name, advisorRequest.Description, advisorRequest.ChangedPicture, null, null);

            return Ok();
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

            return Ok(await RequestToBeAdvisorBusiness.CreateAsync(beAdvisorRequest.Name, beAdvisorRequest.Description, beAdvisorRequest.PreviousExperience));
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

        protected IActionResult RejectRequestToBe(int id)
        {
            RequestToBeAdvisorBusiness.Reject(id);
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

        protected virtual IActionResult ListLastAdvicesForAllTypes(int top)
        {
            return Ok(AdviceBusiness.ListLastAdvicesForAllTypes(top));
        }
    }
}