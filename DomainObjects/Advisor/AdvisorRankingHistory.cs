using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Advisor
{
    public class AdvisorRankingHistory
    {
        [DapperKey]
        [DapperType(System.Data.DbType.Int32)]
        public int UserId { get; set; }
        [DapperKey]
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime ReferenceDate { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int Ranking { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double Rating { get; set; }

        public List<AdvisorProfitHistory> AdvisorProfitHistory { get; set; } = new List<AdvisorProfitHistory>();
    }
}
