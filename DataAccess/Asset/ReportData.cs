using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Asset;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Asset
{
    public class ReportData : BaseSql<Report>, IReportData<Report>
    {
        public override string TableName => "Report";

        private const string SQL_LIST = @"SELECT {0} r.* FROM [Report] r WITH(NOLOCK) {1} {2} ORDER BY r.ReportDate, r.Id DESC";

        public ReportData(IConfigurationRoot configuration) : base(configuration) { }

        public List<Report> ListWithPagination(IEnumerable<int> assetsId, int? top, int? lastReportId)
        {
            DynamicParameters parameters = new DynamicParameters();
            string complement = "";
            string where = "";

            if (assetsId?.Any() == true)
            {
                where = " WHERE ";
                complement = string.Join(" OR ", assetsId.Select((c, i) => $"r.AssetId = @AssetId{i}"));
                for (int i = 0; i < assetsId.Count(); ++i)
                    parameters.Add($"AssetId{i}", assetsId.ElementAt(i), DbType.Int32);
            }

            var topCondition = (top.HasValue ? "TOP " + top.Value : "");
            if (lastReportId.HasValue)
            {
                where = " WHERE ";
                if (!string.IsNullOrEmpty(complement))
                    complement = " ( " + complement + " ) AND ";

                complement += @" (r.ReportDate < (SELECT r2.ReportDate FROM [Report] r2 WITH(NOLOCK) WHERE r2.Id = @LastReportId) 
                                OR (r.ReportDate = (SELECT r2.ReportDate FROM [Report] r2 WITH(NOLOCK) WHERE r2.Id = @LastReportId) AND r.Id < @LastReportId) ) ";
                parameters.Add("LastReportId", lastReportId.Value, DbType.Int32);
            }

            return Query<Report>(string.Format(SQL_LIST, topCondition, where, complement), parameters).ToList();
        }
    }
}
