using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Exchange
{
    public class Pair
    {
        [DapperKey]
        [DapperType(System.Data.DbType.Int32)]
        public int BaseAssetId { get; set; }
        [DapperKey]
        [DapperType(System.Data.DbType.Int32)]
        public int QuoteAssetId { get; set; }
        [DapperKey]
        [DapperType(System.Data.DbType.Int32)]
        public int ExchangeId { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public string Symbol { get; set; }
        [DapperType(System.Data.DbType.Boolean)]
        public bool Enabled { get; set; }
    }
}
