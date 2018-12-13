using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Auctus.DataAccessInterfaces
{
    public interface ITransactionData
    {
        void SetTransaction(IDbConnection connection, IDbTransaction transaction);
        void ClearTransaction();
    }
}
