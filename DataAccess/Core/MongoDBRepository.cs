using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;
using Auctus.Util.NotShared;
using MongoDB.Bson;
using System.Threading.Tasks;
using System.Linq;
using Auctus.DomainObjects;

namespace Auctus.DataAccess.Core
{
    public class MongoDBRepository
    {
        private static MongoClient _client;

        private static MongoClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new MongoClient(Config.MONGO_CONNECTION_STRING);
                }
                return _client;
            }
        }
        private const string DATABASE_NAME = "AucutusPlatform";
        internal static IMongoDatabase GetDataBase()
        {
            IMongoDatabase database = Client.GetDatabase(DATABASE_NAME);
            return database;
        }

        protected Task InsertOneAsync<T>(string collectionName, T document)
        {
            IMongoDatabase database = GetDataBase();
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(collectionName);
            return collection.InsertOneAsync(document.ToBsonDocument());
        }

        protected Task InsertManyAsync<T>(string collectionName, IEnumerable<T> documents)
        {
            IMongoDatabase database = GetDataBase();
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(collectionName);
            return collection.InsertManyAsync(documents.Select(x => x.ToBsonDocument()));
        }

    }
}
