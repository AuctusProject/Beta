using Auctus.DataAccess.Core;
using Auctus.DomainObjects.Account;
using DataAccessInterfaces.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Account
{
    public class FollowData : BaseSQL<Follow>, IFollowData<Follow>
    {
        public override string TableName => "Follow";
    }
}
