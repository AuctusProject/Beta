using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Follow
{
    public class FollowData : BaseSQL<Auctus.DomainObjects.Follow.Follow>
    {
        public override string TableName => "Follow";
    }
}
