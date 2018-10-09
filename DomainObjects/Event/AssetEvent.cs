using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Event
{
    public class AssetEvent
    {
        [DapperKey(true)]
        [DapperType(System.Data.DbType.Int32)]
        public int Id { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public string Title { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public string Description { get; set; }
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime EventDate { get; set; }
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime ExternalCreationDate { get; set; }
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime CreationDate { get; set; }
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime UpdateDate { get; set; }
        [DapperType(System.Data.DbType.Boolean)]
        public bool CanOccurBefore { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public string Proof { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public string Source { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double ReliablePercentage { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public string ExternalId { get; set; }

        public List<LinkEventAsset> LinkEventAsset { get; set; } = new List<LinkEventAsset>();
        public List<LinkEventCategory> LinkEventCategory { get; set; } = new List<LinkEventCategory>();
    }
}
