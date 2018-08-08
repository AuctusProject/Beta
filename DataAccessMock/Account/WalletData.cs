using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Account
{
    public class WalletData : BaseData<Wallet>, IWalletData<Wallet>
    {
        public Wallet GetByAddress(string address)
        {
            throw new NotImplementedException();
        }

        public Wallet GetByUser(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
