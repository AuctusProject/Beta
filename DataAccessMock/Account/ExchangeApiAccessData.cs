using System.Collections.Generic;
using System.Threading.Tasks;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;

namespace Auctus.DataAccessMock.Account
{
    public class ExchangeApiAccessData : BaseData<ExchangeApiAccess>, IExchangeApiAccessData<ExchangeApiAccess>
    {
        public ExchangeApiAccess Get(int userId, int exchangeId)
        {
            throw new System.NotImplementedException();
        }

        public List<ExchangeApiAccess> List(int userId)
        {
            throw new System.NotImplementedException();
        }
    }
}
