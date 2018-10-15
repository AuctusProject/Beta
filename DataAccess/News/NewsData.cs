using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.News;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.News
{
    public class NewsData : BaseSql<DomainObjects.News.News>, INewsData<DomainObjects.News.News>
    {
        public override string TableName => "News";

        private const string SQL_LIST = @"SELECT n.Id, n.Title, n.ExternalCreationDate, n.CreationDate, n.Link,n.SourceId,n.ExternalId FROM 
                                        [News] n WITH(NOLOCK) 
                                        WHERE {0}";


        private const string SQL_LIST_WITH_PAGINATION = @"SELECT {0} n.Id, n.Title, n.ExternalCreationDate, n.CreationDate, n.Link, n.SourceId, n.ExternalId FROM 
                                        [News] n WITH(NOLOCK) 
                                        {1} ORDER BY n.Id DESC";

        public NewsData(IConfigurationRoot configuration) : base(configuration) { }

        public IEnumerable<DomainObjects.News.News> ListNews(IEnumerable<string> externalIds, int sourceId)
        {
            if (externalIds == null && externalIds.Any())
                return new List<DomainObjects.News.News>();

            DynamicParameters parameters = new DynamicParameters();
            var queryCondition = "";

            if (externalIds != null)
            {
                queryCondition = $"({string.Join(" OR ", externalIds.Select((c, i) => $"n.ExternalId = @ExternalId{i}"))})";
                for (int i = 0; i < externalIds.Count(); ++i)
                    parameters.Add($"ExternalId{i}", externalIds.ElementAt(i), DbType.AnsiString);
            }
            
            queryCondition += " AND n.SourceId = @SourceId";
            parameters.Add($"SourceId", sourceId, DbType.Int32);
            
            return Query<DomainObjects.News.News>(string.Format(SQL_LIST, queryCondition), parameters);
        }

        public IEnumerable<DomainObjects.News.News> ListNewsWithPagination(int? top, int? lastNewsId)
        {
            var complement = "";
            var parameters = new DynamicParameters();

            var topCondition = (top.HasValue ? "TOP " + top.Value : String.Empty);
            if (lastNewsId.HasValue)
            {
                if (!string.IsNullOrWhiteSpace(complement))
                {
                    complement = " ( " + complement + " ) ";
                }
                complement += " WHERE n.Id < @LastNewsId ";
                parameters.Add("LastNewsId", lastNewsId.Value, DbType.Int32);
            }

            var query = String.Format(SQL_LIST_WITH_PAGINATION, topCondition, complement);

            return Query<DomainObjects.News.News>(query, parameters);
        }
    }
}
