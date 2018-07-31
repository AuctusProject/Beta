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

                AdvisorServices.Advise(adviseRequest.AssetId, AdviceType.Get(adviseRequest.AdviceType));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            return Ok();
        }
    }
}