using Auctus.DataAccessInterfaces;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Account
{
    public class UserData : BaseData<User>, IUserData<User>
    {
        public User GetByConfirmationCode(string code)
        {
            throw new NotImplementedException();
        }

        public User GetByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public User GetForLogin(string email)
        {
            throw new NotImplementedException();
        }

        public User GetForNewWallet(string email)
        {
            throw new NotImplementedException();
        }
    }
}
