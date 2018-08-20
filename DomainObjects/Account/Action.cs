using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Account
{
    public class Action : MongoDomainObject
    {
        [BsonElement("d")]
        public DateTime CreationDate { get; set; }
        [BsonElement("p")]
        public string Ip { get; set; }
        [BsonElement("u")]
        public int UserId { get; set; }
        [BsonElement("a")]
        public decimal? AucAmount { get; set; }
        [BsonElement("t")]
        public int Type { get; set; }
        [BsonElement("m")]
        public string Message { get; set; }
    }
}
