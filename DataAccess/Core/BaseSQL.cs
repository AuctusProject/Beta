using Auctus.Util;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Auctus.DataAccess.Core;
using Auctus.Util.NotShared;
using System.Threading.Tasks;
using Auctus.DataAccessInterfaces;

namespace Auctus.DataAccess.Core
{
    public abstract class BaseSQL<T> : DapperRepositoryBase, IBaseData<T>
    {
        protected BaseSQL() : base(Config.CONNECTION_STRING)
        { }

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
    }
}
