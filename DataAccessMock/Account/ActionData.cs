using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Account
{
    public class ActionData : BaseData<DomainObjects.Account.Action>, IActionData<DomainObjects.Account.Action>
    {
        public List<DomainObjects.Account.Action> FilterActivity(DateTime startDate, params ActionType[] actionTypes)
        {
            throw new NotImplementedException();
        }

        public override Task InsertOneAsync(DomainObjects.Account.Action obj)
        {
            return Task.FromResult(0);
        }
    }
}
