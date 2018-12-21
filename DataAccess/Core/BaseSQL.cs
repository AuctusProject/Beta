using Auctus.Util;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Auctus.DataAccess.Core;
using System.Threading.Tasks;
using Auctus.DataAccessInterfaces;
using Microsoft.Extensions.Configuration;

namespace Auctus.DataAccess.Core
{
    public abstract class BaseSql<T> : DapperRepositoryBase, IBaseData<T>
    {
        protected BaseSql(IConfigurationRoot configuration) : base(configuration) { }
        
        public void SetTransaction(IDbConnection connection, IDbTransaction transaction)
        {
            base.SetDapperTransaction((SqlConnection)connection, transaction);
        }

        public void ClearTransaction()
        {
            base.ClearDapperTransaction();
        }

        public virtual DateTime GetDateTimeNow()
        {
            return DateTime.UtcNow;
        }

        public IEnumerable<T> SelectAll()
        {
            return base.SelectAll<T>();
        }

        public void Insert(T obj)
        {
            base.Insert<T>(obj);
        }

        public int Update(T obj)
        {
            return base.Update(obj);
        }

        public int Delete(T obj)
        {
            return base.Delete<T>(obj);
        }

        public Task InsertOneAsync(T obj)
        {
            throw new NotImplementedException();
        }

        public Task InsertManyAsync(IEnumerable<T> objs)
        {
            throw new NotImplementedException();
        }

        protected string GetDoubleSqlFormattedValue(double? value)
        {
            if (value.HasValue)
                return value.Value.ToString("##############0.############################", System.Globalization.CultureInfo.InvariantCulture);
            else
                return "NULL";
        }

        protected string GetDateTimeSqlFormattedValue(DateTime? dateTime)
        {
            if (dateTime.HasValue)
                return "'" + dateTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
            else
                return "NULL";
        }

        protected string GetNullableValue(object value)
        {
            if (value != null)
                return value.ToString();
            else
                return "NULL";
        }
    }
}
