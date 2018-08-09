using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;
using Auctus.DomainObjects.Account;

namespace Auctus.DomainObjects.Advisor
{
    public class FollowAdvisor : Follow
    {
        [DapperType(System.Data.DbType.Int32)]
        public int AdvisorId { get; set; }
    }
}
