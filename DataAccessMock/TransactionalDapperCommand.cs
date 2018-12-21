using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using Auctus.DataAccessInterfaces;
using Auctus.DomainObjects.Trade;
using Auctus.DataAccessInterfaces.Trade;
using Auctus.DomainObjects.Advisor;
using Auctus.DataAccessInterfaces.Advisor;

namespace Auctus.DataAccessMock
{
    public class TransactionalDapperCommand : ITransactionalDapperCommand
    {
        protected const int _defaultTimeout = 30;
        protected IServiceProvider ServiceProvider { get; private set; }

        public TransactionalDapperCommand(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        private IBaseData<T> GetData<T>(T obj)
        {
            if (obj is Order)
            {
                return (IOrderData<T>)ServiceProvider.GetService(typeof(IOrderData<T>));
            }
            else if(obj is AdvisorProfit)
            {
                return (IAdvisorProfitData<T>)ServiceProvider.GetService(typeof(IAdvisorProfitData<T>));
            }
            throw new NotImplementedException();
        }

        public new int Delete<T>(T obj, string tableName = null)
        {
            return GetData(obj).Delete(obj);
        }

        public new int Update<T>(T obj, string tableName = null)
        {
            return GetData(obj).Update(obj);
        }

        public new void Insert<T>(T obj, string tableName = null)
        {
            GetData(obj).Insert(obj);
        }

        public void Dispose()
        {
        }

        public void Commit()
        {
        }

        public int Execute(string sql, dynamic param = null, int commandTimeout = _defaultTimeout)
        {
            throw new NotImplementedException();
        }

        public int ExecuteScalar<T>(string sql, dynamic param = null, int commandTimeout = _defaultTimeout)
        {
            throw new NotImplementedException();
        }

        public void SetTransactionOnData(ITransactionData transactionData)
        {
            throw new NotImplementedException();
        }
    }
}
