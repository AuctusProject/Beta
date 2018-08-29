using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessInterfaces.Blockchain
{
    public interface IWeb3Api
    {
        decimal GetAucAmount(string address);
    }
}
