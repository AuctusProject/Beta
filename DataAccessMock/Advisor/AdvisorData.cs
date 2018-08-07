using Auctus.DataAccessInterfaces.Advisor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Advisor
{
    public class AdvisorData : IAdvisorData<DomainObjects.Advisor.Advisor>
    {
        public void Delete(DomainObjects.Advisor.Advisor obj)
        {
            throw new NotImplementedException();
        }

        public void Insert(DomainObjects.Advisor.Advisor obj)
        {
            throw new NotImplementedException();
        }

        public Task InsertManyAsync(IEnumerable<DomainObjects.Advisor.Advisor> objs)
        {
            throw new NotImplementedException();
        }

        public Task InsertOneAsync(DomainObjects.Advisor.Advisor obj)
        {
            throw new NotImplementedException();
        }

        public List<DomainObjects.Advisor.Advisor> ListEnabled()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DomainObjects.Advisor.Advisor> SelectAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DomainObjects.Advisor.Advisor> SelectByObject(DomainObjects.Advisor.Advisor criteria)
        {
            throw new NotImplementedException();
        }

        public void Update(DomainObjects.Advisor.Advisor obj)
        {
            throw new NotImplementedException();
        }
    }
}
