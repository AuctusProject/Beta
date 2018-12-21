using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces;
using Auctus.DomainObjects;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.DataAccess.Core
{
    public abstract class BaseMongo<T> : MongoDBRepository, IBaseData<T>
    {
        public abstract string CollectionName { get; }

        protected BaseMongo(IConfigurationRoot configuration) : base(configuration) { }

        public IMongoCollection<T> Collection
        {
           get
            {
                var database = GetDataBase();
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
        int IBaseData<T>.Delete(T obj)
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

        int IBaseData<T>.Update(T obj)
        {
            throw new NotImplementedException();
        }

        public void SetTransaction(IDbConnection connection, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public void ClearTransaction()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
