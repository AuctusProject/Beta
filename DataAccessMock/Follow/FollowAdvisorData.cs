using Auctus.DataAccessInterfaces.Follow;
using Auctus.DomainObjects.Follow;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Follow
{
    public class FollowAdvisorData : BaseData<FollowAdvisor>, IFollowAdvisorData<FollowAdvisor>
    {
        public List<FollowAdvisor> ListFollowers(IEnumerable<int> advisorIds)
        {
            var id = 0;
            var followers = new List<FollowAdvisor>();
            followers.Add(GetFollowers(ref id, 2, 1));
            followers.Add(GetFollowers(ref id, 3, 1));
            followers.Add(GetFollowers(ref id, 4, 1));
            followers.Add(GetFollowers(ref id, 5, 1));
            followers.Add(GetFollowers(ref id, 3, 2));
            followers.Add(GetFollowers(ref id, 4, 2));
            followers.Add(GetFollowers(ref id, 5, 2));
            followers.Add(GetFollowers(ref id, 1, 3));
            return followers;
        }

        public FollowAdvisor GetLastByUserForAdvisor(int userId, int advisorId)
        {
            throw new NotImplementedException();
        }

        private FollowAdvisor GetFollowers(ref int id, int userId, int advisorId)
        {
            ++id;
            return new FollowAdvisor()
            {
                Id = id,
                UserId = userId,
                AdvisorId = advisorId,
                ActionType = FollowActionType.Follow.Value,
                CreationDate = DateTime.UtcNow
            };
        }
    }
}
