using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Auctus.DataAccessInterfaces
{
    public interface ITransactionalDapperCommand : IDisposable 
    {
        void SetTransactionOnData(ITransactionData transactionData);

        int Delete<T>(T obj, string tableName = null);
        int Update<T>(T obj, string tableName = null);
        void Insert<T>(T obj, string tableName = null);
        void Commit();
        int Execute(string sql, dynamic param, int commandTimeout = 30);
        int ExecuteScalar<T>(string sql, dynamic param, int commandTimeout = 30);
    }
}
