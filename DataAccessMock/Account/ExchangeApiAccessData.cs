using System.Collections.Generic;
using System.Threading.Tasks;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;

namespace Auctus.DataAccessMock.Account
{
    public class ExchangeApiAccessData : IExchangeApiAccessData<ExchangeApiAccess>
    {
        public void Delete(ExchangeApiAccess obj)
        {
            throw new System.NotImplementedException();
        }

        public ExchangeApiAccess Get(int userId, int exchangeId)
        {
            throw new System.NotImplementedException();
        }

        public void Insert(ExchangeApiAccess obj)
        {
            throw new System.NotImplementedException();
        }

        public Task InsertManyAsync(IEnumerable<ExchangeApiAccess> objs)
        {
            throw new System.NotImplementedException();
        }

        public Task InsertOneAsync(ExchangeApiAccess obj)
        {
            throw new System.NotImplementedException();
        }

        public List<ExchangeApiAccess> List(int userId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ExchangeApiAccess> SelectAll()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ExchangeApiAccess> SelectByObject(ExchangeApiAccess criteria)
        {
            throw new System.NotImplementedException();
        }

        public void Update(ExchangeApiAccess obj)
        {
            throw new System.NotImplementedException();
        }
    }
}
