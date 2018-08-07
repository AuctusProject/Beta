using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Advisor
{
    public class RequestToBeAdvisorData : IRequestToBeAdvisorData<RequestToBeAdvisor>
    {
        public void Delete(RequestToBeAdvisor obj)
        {
            throw new NotImplementedException();
        }

        public RequestToBeAdvisor GetByUser(int userId)
        {
            throw new NotImplementedException();
        }

        public void Insert(RequestToBeAdvisor obj)
        {
            throw new NotImplementedException();
        }

        public Task InsertManyAsync(IEnumerable<RequestToBeAdvisor> objs)
        {
            throw new NotImplementedException();
        }

        public Task InsertOneAsync(RequestToBeAdvisor obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RequestToBeAdvisor> SelectAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RequestToBeAdvisor> SelectByObject(RequestToBeAdvisor criteria)
        {
            throw new NotImplementedException();
        }

        public void Update(RequestToBeAdvisor obj)
        {
            throw new NotImplementedException();
        }
    }
}
