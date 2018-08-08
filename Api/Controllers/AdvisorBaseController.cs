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

namespace Api.Controllers
{
    public class AdvisorBaseController : BaseController
    {
        protected AdvisorBaseController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider) : base(loggerFactory, cache, serviceProvider) { } 

        protected IActionResult Advise(AdviseRequest adviseRequest)
        {
            try
            {
                if (adviseRequest == null || adviseRequest.AssetId == 0 || AdviceType.Get(adviseRequest.AdviceType)==null)
                    return BadRequest();

                AdviceBusiness.Advise(adviseRequest.AssetId, AdviceType.Get(adviseRequest.AdviceType));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            return Ok();
        }

        protected async Task<IActionResult> RequestToBe(BeAdvisorRequest beAdvisorRequest)
        {
            try
            {
                if (beAdvisorRequest == null)
                    return BadRequest();

                var result = await RequestToBeAdvisorBusiness.Create(GetUser(), beAdvisorRequest.Name, beAdvisorRequest.Description, beAdvisorRequest.PreviousExperience);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        protected IActionResult GetRequestToBe()
        {
            try
            {
                var result = RequestToBeAdvisorBusiness.GetByEmail(GetUser());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}