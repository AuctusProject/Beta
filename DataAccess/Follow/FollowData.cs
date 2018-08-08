using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Follow;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Follow
{
    public class FollowData : BaseSQL<DomainObjects.Follow.Follow>, IFollowData<DomainObjects.Follow.Follow>
    {
        public override string TableName => "Follow";
    }
}
