using Auctus.DataAccessInterfaces.Follow;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Follow
{
    public class FollowData : IFollowData<DomainObjects.Follow.Follow>
    {
        public void Delete(DomainObjects.Follow.Follow obj)
        {
            throw new NotImplementedException();
        }

        public void Insert(DomainObjects.Follow.Follow obj)
        {
            throw new NotImplementedException();
        }

        public Task InsertManyAsync(IEnumerable<DomainObjects.Follow.Follow> objs)
        {
            throw new NotImplementedException();
        }

        public Task InsertOneAsync(DomainObjects.Follow.Follow obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DomainObjects.Follow.Follow> SelectAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DomainObjects.Follow.Follow> SelectByObject(DomainObjects.Follow.Follow criteria)
        {
            throw new NotImplementedException();
        }

        public void Update(DomainObjects.Follow.Follow obj)
        {
            throw new NotImplementedException();
        }
    }
}
