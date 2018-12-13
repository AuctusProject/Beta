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
using Microsoft.AspNetCore.SignalR;
using Api.Hubs;

namespace Api.Controllers
{
    public class AdvisorBaseController : BaseController
    {
        protected AdvisorBaseController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, IHubContext<AuctusHub> hubContext) :
            base(loggerFactory, cache, serviceProvider, serviceScopeFactory, hubContext) { }

        protected IActionResult GetAdvisor(int id)
        {
            return Ok(AdvisorBusiness.GetAdvisor(id));
        }

        protected IActionResult GetAdvisorDetails(int id)
        {
            return Ok(AdvisorBusiness.GetAdvisorData(id));
        }

        protected IActionResult GetAdvisorPeformance(int id)
        {
            return Ok(AdvisorBusiness.GetAdvisorPeformance(id));
        }

        protected IActionResult GetLoggedAdvisor()
        {
            return Ok(AdvisorBusiness.GetLoggedAdvisor());
        }

        protected IActionResult ListAdvisorsRanking(int? year, int? month)
        {
            return Ok(AdvisorBusiness.ListAdvisorsMonthlyRanking(year, month));
        }

        protected IActionResult ListHallOfFame()
        {
            return Ok(AdvisorMonthlyRankingBusiness.ListHallOfFame());
        }

        protected IActionResult GetAdvisorOrders(int id, int? assetId, int[] status, int? type)
        {
            return Ok(OrderBusiness.ListAdvisorOrders(id, assetId, status, type));
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

        protected async Task<IActionResult> RegisterAsync(RegisterAdvisorRequest beAdvisorRequest)
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
                    response = await AdvisorBusiness.CreateAsync(beAdvisorRequest.Email, beAdvisorRequest.Password, beAdvisorRequest.Name, beAdvisorRequest.Description,
                        beAdvisorRequest.ReferralCode, beAdvisorRequest.ChangedPicture, stream, fileExtension);
            }
            else
                response = await AdvisorBusiness.CreateAsync(beAdvisorRequest.Email, beAdvisorRequest.Password, beAdvisorRequest.Name, beAdvisorRequest.Description,
                    beAdvisorRequest.ReferralCode, beAdvisorRequest.ChangedPicture, null, null);

            return Ok(new { logged = !response.PendingConfirmation, jwt = GenerateToken(response.Email.ToLower().Trim()), data = response });
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