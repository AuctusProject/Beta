using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Auctus.DataAccessInterfaces;
using System.Linq;

namespace Auctus.DataAccess.Core
{
    public class TransactionalDapperCommand : DapperRepositoryBase, ITransactionalDapperCommand
    {
        public override string TableName => throw new NotImplementedException();

        private SqlConnection Connection { get; }
        private IDbTransaction DbTransaction { get; }
        private List<ITransactionData> TransactionData { get; set; } = new List<ITransactionData>();

        public TransactionalDapperCommand(IConfigurationRoot configuration, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) : base(configuration)
        {
            Connection = CreateConnection();
            DbTransaction = Connection.BeginTransaction(isolationLevel);
        }

        public void SetTransactionOnData(ITransactionData transactionData)
        {
            TransactionData.Add(transactionData);
            transactionData.SetTransaction(Connection, DbTransaction);
        }

        public new int Delete<T>(T obj, string tableName = null)
        {
            return base.Delete<T>(obj, GetTableName<T>(tableName), true);
        }

        public new int Update<T>(T obj, string tableName = null)
        {
            return base.Update<T>(obj, GetTableName<T>(tableName), true);
        }

        public new void Insert<T>(T obj, string tableName = null)
        {
            base.Insert<T>(obj, GetTableName<T>(tableName));
        }

        private string GetTableName<T>(string tableName)
        {
            return string.Format("{0}", tableName ?? typeof(T).Name);
        }

        public void Dispose()
        {
            DbTransaction.Dispose();
            Connection.Dispose();
            Connection.Close();
            if (TransactionData != null && TransactionData.Any())
            {
                foreach(var transactionData in TransactionData)
                    transactionData.ClearTransaction();

                TransactionData = new List<ITransactionData>();
            }
        }

        public void Commit()
        {
            try
            {
                DbTransaction.Commit();
            }
            catch
            {
                DbTransaction.Rollback();
                throw;
            }
        }

        public int Execute(string sql, dynamic param = null, int commandTimeout = _defaultTimeout)
        {
            return this.Execute(sql, param, commandTimeout, DbTransaction);
        }

        public int ExecuteScalar<T>(string sql, dynamic param = null, int commandTimeout = _defaultTimeout)
        {
            try
            {
                return SqlMapper.ExecuteScalar<T>(Connection, sql, param, DbTransaction, commandTimeout, CommandType.Text);
            }
            catch
            {
                DbTransaction.Rollback();
                throw;
            }
        }

        protected override int Execute(string sql, dynamic param = null, int commandTimeout = _defaultTimeout, IDbTransaction transaction = null)
        {
            try
            {
                return SqlMapper.Execute(Connection, sql, param, DbTransaction, commandTimeout, CommandType.Text);
            }
            catch
            {
                DbTransaction.Rollback();
                throw;
            }
        }

        protected override int ExecuteReturningIdentity(string sql, dynamic param = null, int commandTimeout = _defaultTimeout, IDbTransaction transaction = null)
        {
            try
            {
                return SqlMapper.ExecuteScalar<int>(Connection, ParseIdentityCommandQuery(sql), param, DbTransaction, commandTimeout, CommandType.Text);
            }
            catch
            {
                DbTransaction.Rollback();
                throw;
            }
        }
    }
}
