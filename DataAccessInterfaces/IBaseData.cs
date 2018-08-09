using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessInterfaces
{
    public interface IBaseData<T>
    {
        IEnumerable<T> SelectByObject(T criteria);
        IEnumerable<T> SelectAll();
        void Insert(T obj);
        void Update(T obj);
        void Delete(T obj);
        Task InsertOneAsync(T obj);
        Task InsertManyAsync(IEnumerable<T> objs);

        DateTime GetDateTimeNow();
    }
}
