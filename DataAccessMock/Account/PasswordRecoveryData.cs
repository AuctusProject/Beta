using System.Collections.Generic;
using System.Threading.Tasks;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;

namespace Auctus.DataAccessMock.Account
{
    public class PasswordRecoveryData : IPasswordRecoveryData<PasswordRecovery>
    {
        public void Delete(PasswordRecovery obj)
        {
            throw new System.NotImplementedException();
        }

        public void Insert(PasswordRecovery obj)
        {
            throw new System.NotImplementedException();
        }

        public Task InsertManyAsync(IEnumerable<PasswordRecovery> objs)
        {
            throw new System.NotImplementedException();
        }

        public Task InsertOneAsync(PasswordRecovery obj)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<PasswordRecovery> SelectAll()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<PasswordRecovery> SelectByObject(PasswordRecovery criteria)
        {
            throw new System.NotImplementedException();
        }

        public void Update(PasswordRecovery obj)
        {
            throw new System.NotImplementedException();
        }
    }
}
