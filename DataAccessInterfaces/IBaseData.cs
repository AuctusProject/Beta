using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Auctus.DataAccessInterfaces
{
    public interface IBaseData<T> : ITransactionData
    {
        IEnumerable<T> SelectAll();
        void Insert(T obj);
        int Update(T obj);
        int Delete(T obj);
        Task InsertOneAsync(T obj);
        Task InsertManyAsync(IEnumerable<T> objs);

        DateTime GetDateTimeNow();
    }
}
