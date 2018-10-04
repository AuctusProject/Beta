using Auctus.DomainObjects.Asset;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessInterfaces.Asset
{
    public interface IReportData<T> : IBaseData<T>
    {
        List<Report> ListWithPagination(IEnumerable<int> assetsId, int? top, int? lastReportId);
    }
}
