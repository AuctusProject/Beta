using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Event;
using Auctus.DomainObjects.Event;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Event
{
    public class AssetEventData : BaseSql<AssetEvent>, IAssetEventData<AssetEvent>
    {
        public override string TableName => "AssetEvent";

        private const string SQL_LIST = @"SELECT e.*, a.*, c.* FROM 
                                        [AssetEvent] e WITH(NOLOCK) 
                                        INNER JOIN [LinkEventAsset] a WITH(NOLOCK) ON a.AssetEventId = e.Id
                                        INNER JOIN [LinkEventCategory] c WITH(NOLOCK) ON c.AssetEventId = e.Id
                                        {0} {1} ORDER BY e.ExternalCreationDate DESC";

        public AssetEventData(IConfigurationRoot configuration) : base(configuration) { }

        public List<AssetEvent> ListAssetEventsWithPagination(DateTime? startDate, IEnumerable<int> assetsId, int? top, int? lastAssetEventId, double? minimumReliablePercentage, bool? onlyFutureEvents)
        {
            if (assetsId != null && !assetsId.Any())
                return new List<AssetEvent>();

            DynamicParameters parameters = new DynamicParameters();
            var where = assetsId != null || lastAssetEventId.HasValue || startDate.HasValue || minimumReliablePercentage.HasValue || onlyFutureEvents.HasValue ? " WHERE " : "";
            var queryCondition = "";
            
            if (assetsId != null)
            {
                queryCondition = $"({string.Join(" OR ", assetsId.Select((c, i) => $"a.AssetId = @AssetId{i}"))})";
                for (int i = 0; i < assetsId.Count(); ++i)
                    parameters.Add($"AssetId{i}", assetsId.ElementAt(i), DbType.Int32);
            }

            if (startDate.HasValue)
            {
                if (!string.IsNullOrEmpty(queryCondition))
                    queryCondition += " AND ";

                queryCondition += "e.EventDate >= @StartDate";
                parameters.Add($"StartDate", startDate.Value, DbType.DateTime);
            }

            if (lastAssetEventId.HasValue)
            {
                if (!string.IsNullOrEmpty(queryCondition))
                    queryCondition += " AND ";

                queryCondition += "e.ExternalCreationDate < (SELECT e2.ExternalCreationDate FROM [AssetEvent] e2 WITH(NOLOCK) WHERE e2.Id = @LastAssetEventId)";
                parameters.Add("LastAssetEventId", lastAssetEventId.Value, DbType.Int32);
            }

            if (minimumReliablePercentage.HasValue)
            {
                if (!string.IsNullOrEmpty(queryCondition))
                    queryCondition += " AND ";

                queryCondition += "e.ReliablePercentage >= @ReliablePercentage";
                parameters.Add("ReliablePercentage", minimumReliablePercentage.Value, DbType.Double);
            }

            if (onlyFutureEvents.HasValue)
            {
                if (!string.IsNullOrEmpty(queryCondition))
                    queryCondition += " AND ";

                queryCondition += "e.EventDate >= @Today";
                parameters.Add("Today", GetDateTimeNow().Date, DbType.DateTime);
            }

            Dictionary<int, AssetEvent> cache = new Dictionary<int, AssetEvent>();
            Query<AssetEvent, LinkEventAsset, LinkEventCategory, AssetEvent>(string.Format(SQL_LIST, where, queryCondition),
                (assetEvent, linkEventAsset, linkEventCategory) =>
                {

                    if (!cache.ContainsKey(assetEvent.Id))
                        cache.Add(assetEvent.Id, assetEvent);

                    var cachedParent = cache[assetEvent.Id];
                    if (linkEventAsset != null && !cachedParent.LinkEventAsset.Any(c => c.AssetId == linkEventAsset.AssetId))
                        cachedParent.LinkEventAsset.Add(linkEventAsset);
                    if (linkEventCategory != null && !cachedParent.LinkEventCategory.Any(c => c.AssetEventCategoryId == linkEventCategory.AssetEventCategoryId))
                        cachedParent.LinkEventCategory.Add(linkEventCategory);

                    return cachedParent;
                }, "AssetEventId,AssetEventId", parameters);

            if (top.HasValue)
                return cache.Values.Take(top.Value).ToList();
            else
                return cache.Values.ToList();
        }
    }
}
