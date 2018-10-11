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

        private const string SQL_LIST = @"SELECT n.* FROM 
                                        [News] n WITH(NOLOCK) 
                                        WHERE {0}";

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
    }
}
