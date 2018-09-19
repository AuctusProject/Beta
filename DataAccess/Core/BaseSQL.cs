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

        public IEnumerable<T> SelectByObject(T criteria)
        {
            return base.SelectByObject<T>(criteria);
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

        public void Update(T obj)
        {
            base.Update(obj);
        }

        public void Delete(T obj)
        {
            base.Delete<T>(obj);
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
                return dateTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff");
            else
                return "NULL";
        }
    }
}
