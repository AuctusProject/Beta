using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Advisor
{
    public class AdviceData : IAdviceData<Advice>
    {
        public void Delete(Advice obj)
        {
            throw new System.NotImplementedException();
        }

        public void Insert(Advice obj)
        {
            throw new System.NotImplementedException();
        }

        public Task InsertManyAsync(IEnumerable<Advice> objs)
        {
            throw new System.NotImplementedException();
        }

        public Task InsertOneAsync(Advice obj)
        {
            throw new System.NotImplementedException();
        }

        public List<Advice> List(IEnumerable<int> advisorIds)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Advice> SelectAll()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Advice> SelectByObject(Advice criteria)
        {
            throw new System.NotImplementedException();
        }

        public void Update(Advice obj)
        {
            throw new System.NotImplementedException();
        }
    }
}
