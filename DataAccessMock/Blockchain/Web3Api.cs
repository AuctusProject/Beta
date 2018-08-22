using Auctus.DataAccessInterfaces.Blockchain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Blockchain
{
    public class Web3Api : IWeb3Api
    {
        public decimal GetAucAmount(string address)
        {
            throw new NotImplementedException();
        }
    }
}
