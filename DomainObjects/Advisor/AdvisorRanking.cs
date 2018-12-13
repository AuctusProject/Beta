using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Advisor
{
    public class AdvisorRanking : Advisor
    {
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime UpdateDate { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int Ranking { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double Rating { get; set; }

        public List<AdvisorProfit> AdvisorProfit { get; set; } = new List<AdvisorProfit>();
        public int TotalAdvisors { get; set; }
    }
}
