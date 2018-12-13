using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Advisor
{
    public class AdvisorMonthlyRanking
    {
        [DapperType(System.Data.DbType.Int32)]
        public int UserId { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int Year { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int Month { get; set; }
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime CreationDate { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int Ranking { get; set; }
        [DapperType(System.Data.DbType.Double)]
        public double AverageReturn { get; set; }
    }
}
