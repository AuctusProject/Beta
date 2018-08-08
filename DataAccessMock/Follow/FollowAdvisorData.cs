using Auctus.DataAccessInterfaces.Follow;
using Auctus.DomainObjects.Follow;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Follow
{
    public class FollowAdvisorData : BaseData<FollowAdvisor>, IFollowAdvisorData<FollowAdvisor>
    {
        public List<FollowAdvisor> List(IEnumerable<int> advisorIds)
        {
            throw new NotImplementedException();
        }

        public FollowAdvisor GetLastByUserForAdvisor(int userId, int advisorId)
        {
            throw new NotImplementedException();
        }
    }
}
