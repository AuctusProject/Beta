using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Asset;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Asset
{
    public class ReportData : BaseSql<Report>, IReportData<Report>
    {
        public override string TableName => "Report";

        public ReportData(IConfigurationRoot configuration) : base(configuration) { }
    }
}
