using Auctus.DataAccess.Core;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Follow;
using Auctus.DataAccess.Follow;

namespace Auctus.Business.Follow
{
    public class FollowBusiness : BaseBusiness<Auctus.DomainObjects.Follow.Follow, FollowData>
    {
        public FollowBusiness(ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(loggerFactory, cache, email, ip) { }
    }
}
