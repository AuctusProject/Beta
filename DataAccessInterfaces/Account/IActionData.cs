using Auctus.DomainObjects.Account;
using System;
using System.Collections.Generic;

namespace Auctus.DataAccessInterfaces.Account
{
    public interface IActionData<T> : IBaseData<T>
    {
        List<DomainObjects.Account.Action> FilterActivity(DateTime startDate, params ActionType[] actionTypes);
    }
}