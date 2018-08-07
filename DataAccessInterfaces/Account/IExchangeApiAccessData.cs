using Auctus.DomainObjects.Account;
using DataAccessInterfaces;
using System.Collections.Generic;

namespace Auctus.DataAccessInterfaces.Account
{
    public interface IExchangeApiAccessData<T> : IBaseData<T>
    {
        List<ExchangeApiAccess> List(int userId);
        ExchangeApiAccess Get(int userId, int exchangeId);
    }
}