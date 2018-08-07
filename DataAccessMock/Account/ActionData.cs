using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Account
{
    public class ActionData : IActionData<DomainObjects.Account.Action>
    {
        public void Delete(DomainObjects.Account.Action obj)
        {
            throw new NotImplementedException();
        }

        public void Insert(DomainObjects.Account.Action obj)
        {
            throw new NotImplementedException();
        }

        public Task InsertManyAsync(IEnumerable<DomainObjects.Account.Action> objs)
        {
            throw new NotImplementedException();
        }

        public Task InsertOneAsync(DomainObjects.Account.Action obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DomainObjects.Account.Action> SelectAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DomainObjects.Account.Action> SelectByObject(DomainObjects.Account.Action criteria)
        {
            throw new NotImplementedException();
        }

        public void Update(DomainObjects.Account.Action obj)
        {
            throw new NotImplementedException();
        }
    }
}
