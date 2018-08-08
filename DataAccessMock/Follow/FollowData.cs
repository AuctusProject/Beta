using Auctus.DataAccessInterfaces.Follow;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Follow
{
    public class FollowData : BaseData<DomainObjects.Follow.Follow>, IFollowData<DomainObjects.Follow.Follow>
    {
    }
}
