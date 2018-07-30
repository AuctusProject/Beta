using Auctus.DataAccess.Follow;
using Auctus.DomainObjects.Follow;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Follow
{
    public class FollowAdvisorBusiness : BaseBusiness<FollowAdvisor, FollowAdvisorData>
    {
        public FollowAdvisorBusiness(ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(loggerFactory, cache, email, ip) { }
    }
}
