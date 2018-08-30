using Auctus.DataAccessInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock
{
    public class BaseData<T> : IBaseData<T>
    {
        public virtual DateTime GetDateTimeNow()
        {
            return new DateTime(2018, 6, 30, 23, 55, 0);
        }

        public virtual void Delete(T obj)
        {
            throw new NotImplementedException();
        }

        public virtual void Insert(T obj)
        {
            throw new NotImplementedException();
        }

        public virtual Task InsertManyAsync(IEnumerable<T> objs)
        {
            throw new NotImplementedException();
        }

        public virtual Task InsertOneAsync(T obj)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<T> SelectAll()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<T> SelectByObject(T criteria)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(T obj)
        {
            throw new NotImplementedException();
        }
    }
}
