using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DataAccessMock;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Advisor
{
    public class FollowAdvisorData : BaseData<FollowAdvisor>, IFollowAdvisorData<FollowAdvisor>
    {
        public static List<FollowAdvisor> FollowAdvisorList = CreateFollowAdvisorList();

        private static List<FollowAdvisor> CreateFollowAdvisorList()
        {
            var id = 0;
            return new List<FollowAdvisor>
            {
                GetFollowers(ref id, 2, 1),
                GetFollowers(ref id, 3, 1),
                GetFollowers(ref id, 4, 1),
                GetFollowers(ref id, 5, 1),
                GetFollowers(ref id, 3, 2),
                GetFollowers(ref id, 4, 2),
                GetFollowers(ref id, 5, 2),
                GetFollowers(ref id, 1, 3)
            };
        }

        public List<FollowAdvisor> ListFollowers(IEnumerable<int> advisorIds)
        {
            return FollowAdvisorList;
        }

        public FollowAdvisor GetLastByUserForAdvisor(int userId, int advisorId)
        {
            throw new NotImplementedException();
        }

        private static FollowAdvisor GetFollowers(ref int id, int userId, int advisorId)
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
