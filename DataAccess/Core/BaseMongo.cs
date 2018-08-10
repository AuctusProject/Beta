using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces;
using Auctus.DomainObjects;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.DataAccess.Core
{
    public abstract class BaseMongo<T> : MongoDBRepository, IBaseData<T>
    {
        public abstract string CollectionName { get; }

        public IMongoCollection<T> Collection
        {
           get
            {
                var database = MongoDBRepository.GetDataBase();
                var collection = database.GetCollection<T>(CollectionName);
                return collection;
            }
        }

        public virtual DateTime GetDateTimeNow()
        {
            return DateTime.UtcNow;
        }

        public Task InsertOneAsync(T obj)
        {
            return base.InsertOneAsync(CollectionName, obj);
        }

        public Task InsertManyAsync(IEnumerable<T> objs)
        {
            return base.InsertManyAsync(CollectionName, objs);
        }

        #region IBaseData implementation
        void IBaseData<T>.Delete(T obj)
        {
            throw new NotImplementedException();
        }

        void IBaseData<T>.Insert(T obj)
        {
            throw new NotImplementedException();
        }

        IEnumerable<T> IBaseData<T>.SelectAll()
        {
            throw new NotImplementedException();
        }

        IEnumerable<T> IBaseData<T>.SelectByObject(T criteria)
        {
            throw new NotImplementedException();
        }

        void IBaseData<T>.Update(T obj)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
