using Auctus.DataAccess.Core;
using Auctus.DomainObjects.Advisor;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Follow;
using Auctus.DataAccess.Follow;

namespace Auctus.Business.Advisor
{
    public class FollowBusiness : BaseBusiness<Follow, FollowData>
    {
        public FollowBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }
    }
}
