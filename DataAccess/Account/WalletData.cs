using Auctus.DataAccess.Core;
using Auctus.DomainObjects.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Account
{
    public class WalletData : BaseSQL<Wallet>
    {
        public override string TableName => "Wallet";
    }
}
