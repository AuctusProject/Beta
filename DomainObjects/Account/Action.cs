using Auctus.Util.DapperAttributes;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Account
{
    public class Action //: MongoDomainObject
    {
        [DapperKey(true)]
        [DapperType(System.Data.DbType.Int32)]
        public int Id { get; set; }
        //[BsonElement("d")]
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime CreationDate { get; set; }
        //[BsonElement("p")]
        [DapperType(System.Data.DbType.AnsiString)]
        public string Ip { get; set; }
        //[BsonElement("u")]
        [DapperType(System.Data.DbType.Int32)]
        public int UserId { get; set; }
        //[BsonElement("a")]
        [DapperType(System.Data.DbType.Double)]
        public decimal? AucAmount { get; set; }
        //[BsonElement("t")]
        [DapperType(System.Data.DbType.Int32)]
        public int Type { get; set; }
        //[BsonElement("m")]
        [DapperType(System.Data.DbType.AnsiString)]
        public string Message { get; set; }
    }
}
