using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Asset
{
    public class Report
    {
        [DapperKey(true)]
        [DapperType(System.Data.DbType.Int32)]
        public int Id { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int AssetId { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int AgencyId { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int AgencyRatingId { get; set; }
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime CreationDate { get; set; }
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime ReportDate { get; set; }

        public Agency Agency { get; set; }
        public AgencyRating AgencyRating { get; set; }
    }
}
