using Auctus.DataAccessInterfaces.Follow;
using Auctus.DomainObjects.Follow;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Follow
{
    public class FollowAdvisorData : IFollowAdvisorData<FollowAdvisor>
    {
        public void Delete(FollowAdvisor obj)
        {
            throw new NotImplementedException();
        }

        public void Insert(FollowAdvisor obj)
        {
            throw new NotImplementedException();
        }

        public Task InsertManyAsync(IEnumerable<FollowAdvisor> objs)
        {
            throw new NotImplementedException();
        }

        public Task InsertOneAsync(FollowAdvisor obj)
        {
            throw new NotImplementedException();
        }

        public List<FollowAdvisor> List(IEnumerable<int> advisorIds)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FollowAdvisor> SelectAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FollowAdvisor> SelectByObject(FollowAdvisor criteria)
        {
            throw new NotImplementedException();
        }

        public void Update(FollowAdvisor obj)
        {
            throw new NotImplementedException();
        }

        public FollowAdvisor GetLastByUserForAdvisor(int userId, int advisorId)
        {
            throw new NotImplementedException();
        }
    }
}
