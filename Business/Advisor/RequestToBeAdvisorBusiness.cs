using Auctus.DataAccess.Advisor;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
using Auctus.Util;
using Auctus.Util.NotShared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.Business.Advisor
{
    public class RequestToBeAdvisorBusiness : BaseBusiness<RequestToBeAdvisor, RequestToBeAdvisorData>
    {
        public RequestToBeAdvisorBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }
    }
}
