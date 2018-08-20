using Auctus.Util.DapperAttributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Asset
{
    public class AssetValue : MongoDomainObject
    {
        [BsonElement("a")]
        public int AssetId { get; set; }
        [BsonElement("d")]
        public DateTime Date { get; set; }
        [BsonElement("v")]
        public double Value { get; set; }
        [BsonElement("m")]
        public double? MarketCap { get; set; }
    }
}
