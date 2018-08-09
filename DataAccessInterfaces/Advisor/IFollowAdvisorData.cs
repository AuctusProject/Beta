using Auctus.DataAccessInterfaces;
using Auctus.DomainObjects.Advisor;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessInterfaces.Advisor
{
    public interface IFollowAdvisorData<T> : IBaseData<T>
    {
        List<FollowAdvisor> ListFollowers(IEnumerable<int> advisorIds);
        FollowAdvisor GetLastByUserForAdvisor(int userId, int advisorId);
    }
}
