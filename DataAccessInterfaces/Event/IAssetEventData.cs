using Auctus.DomainObjects.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessInterfaces.Event
{
    public interface IAssetEventData<AssetEvent> : IBaseData<AssetEvent>
    {
        List<AssetEvent> ListAssetEventsWithPagination(DateTime? startDate, IEnumerable<int> assetsId, int? top, int? lastAssetEventId, double? minimumReliablePercentage, bool? onlyFutureEvents);
    }
}
