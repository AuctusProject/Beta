using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Asset;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Asset
{
    public class ReportData : BaseData<Report>, IReportData<Report>
    {
        public List<Report> ListWithPagination(IEnumerable<int> assetsId, int? top, int? lastReportId)
        {
            throw new NotImplementedException();
        }
    }
}
