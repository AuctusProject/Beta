using Auctus.DomainObjects.Advisor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessInterfaces.Advisor
{
    public interface IFollowAdvisorData<T> : IBaseData<T>
    {
        List<FollowAdvisor> ListFollowers(IEnumerable<int> advisorIds, bool includeUserData);
        FollowAdvisor GetLastByUserForAdvisor(int userId, int advisorId);
        List<FollowAdvisor> ListAdvisorsFollowed(int userId);
        int CountAdvisorFollowers(int advisorId);
    }
}
