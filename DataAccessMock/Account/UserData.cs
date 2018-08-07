using Auctus.DataAccessInterfaces;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Account
{
    public class UserData : IUserData<User>
    {
        public void Delete(User obj)
        {
            throw new NotImplementedException();
        }

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

        public void Insert(User obj)
        {
            throw new NotImplementedException();
        }

        public Task InsertManyAsync(IEnumerable<User> objs)
        {
            throw new NotImplementedException();
        }

        public Task InsertOneAsync(User obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> SelectAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> SelectByObject(User criteria)
        {
            throw new NotImplementedException();
        }

        public void Update(User obj)
        {
            throw new NotImplementedException();
        }
    }
}
