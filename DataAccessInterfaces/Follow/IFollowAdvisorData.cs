using Auctus.DomainObjects.Follow;
using System.Collections.Generic;

namespace Auctus.DataAccessInterfaces.Follow
{
    public interface IFollowAdvisorData<T> : IBaseData<T>
    {
        List<FollowAdvisor> ListFollowers(IEnumerable<int> advisorIds);
        FollowAdvisor GetLastByUserForAdvisor(int userId, int advisorId);
    }
}