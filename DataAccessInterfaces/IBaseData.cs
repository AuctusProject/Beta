using System.Collections.Generic;

namespace DataAccessInterfaces
{
    public interface IBaseData<T>
    {
        IEnumerable<T> SelectByObject(T criteria);
        IEnumerable<T> SelectAll();
        void Insert(T obj);
        void Update(T obj);
        void Delete(T obj);
    }
}
