using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Auctus.DataAccessInterfaces
{
    public interface IBaseData<T> : ITransactionData
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
