using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Asset;
using Dapper;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Auctus.DataAccess.Asset
{
    public class AssetValueData : BaseMongo<AssetValue>, IAssetValueData<AssetValue>
    {
        public override string CollectionName => "AssetValue";
        public AssetValueData(IConfigurationRoot configuration) : base(configuration) { }

        public AssetValue GetLastValue(int assetId)
        {
            return Collection.Find(x => x.AssetId == assetId).SortByDescending(x => x.Date).FirstOrDefault(); 
        }

        public List<AssetValue> FilterAssetValues(Dictionary<int, DateTime> assetsMap)
        {
            var filterBuilder = Builders<AssetValue>.Filter;
            FilterDefinition<AssetValue> filter = null;
            foreach (KeyValuePair<int, DateTime> pair in assetsMap)
            {
                if (filter == null)
                    filter = (filterBuilder.Eq(asset => asset.AssetId, pair.Key) & filterBuilder.Gte(asset => asset.Date, pair.Value));
                else
                    filter = filter | (filterBuilder.Eq(asset => asset.AssetId, pair.Key) & filterBuilder.Gte(asset => asset.Date, pair.Value));
            }
            var options = new FindOptions()
            {
                BatchSize = 99999999,
                NoCursorTimeout = true
            };
            return Collection.Find(filter, options).ToList();
        }
    }
}
