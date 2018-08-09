using Auctus.DataAccessMock;
using Auctus.DomainObjects.Account;
using DataAccessInterfaces.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessMock.Account
{
    public class FollowData : BaseData<Follow>, IFollowData<Follow>
    {
    }
}
