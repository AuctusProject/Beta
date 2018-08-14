using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;
using Auctus.Util.NotShared;
using MongoDB.Bson;
using System.Threading.Tasks;
using System.Linq;
using Auctus.DomainObjects;
using Microsoft.Extensions.Configuration;

namespace Auctus.DataAccess.Core
{
    public abstract class MongoDBRepository
    {
        private const string DATABASE_NAME = "AucutusPlatform";
        protected readonly IConfigurationRoot Configuration;

        protected MongoDBRepository(IConfigurationRoot configuration)
        {
            Configuration = configuration;
            MongoConnection.Initiate(configuration.GetSection("ConnectionString:Mongo").Get<string>());
        }

        protected IMongoDatabase GetDataBase()
        {
            return MongoConnection.Client.GetDatabase(DATABASE_NAME);
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

        private static class MongoConnection
        {
            internal static MongoClient Client { get; private set; }

            internal static void Initiate(string connectionString)
            {
                if (Client == null)
                    Client = new MongoClient(connectionString);
            }
        }
    }
}
