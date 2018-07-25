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
    [Route("api/advisors/v1/")]
    [Authorize("Bearer")]
    [EnableCors("Default")]
    public class AdvisorV1Controller : AdvisorBaseController
    {
        public AdvisorV1Controller(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider) : base(loggerFactory, cache, serviceProvider) { }
        
    }
}