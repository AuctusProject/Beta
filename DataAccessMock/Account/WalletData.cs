using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Account
{
    public class WalletData : IWalletData<Wallet>
    {
        public void Delete(Wallet obj)
        {
            throw new NotImplementedException();
        }

        public Wallet GetByAddress(string address)
        {
            throw new NotImplementedException();
        }

        public Wallet GetByUser(int userId)
        {
            throw new NotImplementedException();
        }

        public void Insert(Wallet obj)
        {
            throw new NotImplementedException();
        }

        public Task InsertManyAsync(IEnumerable<Wallet> objs)
        {
            throw new NotImplementedException();
        }

        public Task InsertOneAsync(Wallet obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Wallet> SelectAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Wallet> SelectByObject(Wallet criteria)
        {
            throw new NotImplementedException();
        }

        public void Update(Wallet obj)
        {
            throw new NotImplementedException();
        }
    }
}
