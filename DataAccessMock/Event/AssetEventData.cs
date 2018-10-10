using Auctus.DataAccessInterfaces.Event;
using Auctus.DomainObjects.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Event
{
    public class AssetEventData : BaseData<AssetEvent>, IAssetEventData<AssetEvent>
    {
        public List<AssetEvent> ListAssetEventsWithPagination(DateTime? startDate, IEnumerable<int> assetsId, int? top, int? lastAssetEventId, double? minimumReliablePercentage, bool? onlyFutureEvents)
        {
            return new List<AssetEvent>();
        }
    }
}
