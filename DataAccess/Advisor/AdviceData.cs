using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Advisor
{
    public class AdviceData : BaseSql<Advice>, IAdviceData<Advice>
    {
        public override string TableName => "Advice";
        public AdviceData(IConfigurationRoot configuration) : base(configuration) { }

        private const string SQL_LIST = @"SELECT a.* FROM [Advice] a WITH(NOLOCK) WHERE {0}";

        private const string SQL_GET_LAST_FOR_ASSET_BY_ADVISOR = @"
	    SELECT TOP 1
		    a.*
	    FROM 
    	    [Advice] a WITH(NOLOCK)
        WHERE 
    	    a.AssetId = @AssetId
            AND a.AdvisorId = @AdvisorId
        ORDER BY a.CreationDate DESC ";

        private const string SQL_LIST_LAST_ADVICES_FOR_USER_WITH_PAGINATION = @"
	    SELECT {0} a.*
            FROM [Advice] a WITH(NOLOCK)
            INNER JOIN Advisor WITH(NOLOCK) ON Advisor.Id = a.AdvisorId AND Advisor.Enabled = 1
            WHERE 
            {1}
            ORDER BY Id DESC";

        private const string SQL_LIST_LAST_BY_TYPE = @"
	    SELECT * FROM
		    (SELECT {0}
			    a.* 
		    FROM 
			    Advice a WITH(NOLOCK)
		    WHERE
			    a.Type = @Type{1}
		    ORDER BY a.CreationDate DESC
		    ) a{1}";

        private const string SQL_LIST_TRENDING_ADVISED_ASSETS = @"
                SELECT top {0} 
                    assetId 
                FROM 
                    (SELECT TOP 1000 
                        * 
                    FROM 
                        Advice WITH(NOLOCK) 
                    ORDER BY 
                    creationdate DESC
                    ) a
                GROUP BY 
                    assetId 
                ORDER BY 
                    count(*) DESC";


        public List<Advice> List(IEnumerable<int> advisorIds = null, IEnumerable<int> assetsIds = null)
        {
            if ((!advisorIds?.Any() ?? true) && (!assetsIds?.Any() ?? true))
                throw new ArgumentNullException("advisorIds");

            var complement = "";
            DynamicParameters parameters = new DynamicParameters();
            if (advisorIds?.Any() ?? false)
            {
                complement = $"({string.Join(" OR ", advisorIds.Select((c, i) => $"a.AdvisorId = @AdvisorId{i}"))})";
                for (int i = 0; i < advisorIds.Count(); ++i)
                    parameters.Add($"AdvisorId{i}", advisorIds.ElementAt(i), DbType.Int32);
            }
            if (assetsIds?.Any() ?? false)
            {
                if (advisorIds?.Any() ?? false)
                    complement += " AND ";
                complement += $"({string.Join(" OR ", assetsIds.Select((c, i) => $"a.AssetId = @AssetId{i}"))})";
                for (int i = 0; i < assetsIds.Count(); ++i)
                    parameters.Add($"AssetId{i}", assetsIds.ElementAt(i), DbType.Int32);
            }
            return Query<Advice>(string.Format(SQL_LIST, complement), parameters).ToList();
        }

        public Advice GetLastAdviceForAssetByAdvisor(int assetId, int advisorId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("AssetId", assetId, DbType.Int32);
            parameters.Add("AdvisorId", advisorId, DbType.Int32);

            return Query<Advice>(SQL_GET_LAST_FOR_ASSET_BY_ADVISOR, parameters).SingleOrDefault();
        }

        public IEnumerable<Advice> ListLastAdvicesWithPagination(IEnumerable<int> followingAdvisors, IEnumerable<int> followingAssets, int? top, int? lastAdviceId)
        {
            if (!followingAdvisors.Any() && !followingAssets.Any())
                return Enumerable.Empty<Advice>();

            var complement = "";
            var parameters = new DynamicParameters();
            if (followingAdvisors.Any())
            {
                complement += string.Join(" OR ", followingAdvisors.Select((c, i) => $"a.AdvisorId = @AdvisorId{i}"));
                for (int i = 0; i < followingAdvisors.Count(); ++i)
                    parameters.Add($"AdvisorId{i}", followingAdvisors.ElementAt(i), DbType.Int32);
            }
            if (followingAssets.Any())
            {
                if (!String.IsNullOrWhiteSpace(complement))
                {
                    complement += " OR ";
                }

                complement += string.Join(" OR ", followingAssets.Select((c, i) => $"a.AssetId = @AssetId{i}"));
                for (int i = 0; i < followingAssets.Count(); ++i)
                    parameters.Add($"AssetId{i}", followingAssets.ElementAt(i), DbType.Int32);
            }

            var topCondition = (top.HasValue ? "TOP " + top.Value : String.Empty);
            if (lastAdviceId.HasValue) {
                if (!string.IsNullOrWhiteSpace(complement))
                {
                    complement = " ( " + complement + " ) ";
                }
                complement += " AND a.Id < @LastAdviceId ";
                parameters.Add("LastAdviceId", lastAdviceId.Value, DbType.Int32);
            }

            var query = String.Format(SQL_LIST_LAST_ADVICES_FOR_USER_WITH_PAGINATION, topCondition, complement);

            return Query<Advice>(query, parameters);
        }

        public IEnumerable<Advice> ListLastAdvicesForAllTypes(int? top)
        {
            var topCondition = (top.HasValue ? "TOP " + top.Value : String.Empty);
            var validTypes = new AdviceType[] { AdviceType.Sell, AdviceType.Buy, AdviceType.ClosePosition };
            var parameters = new DynamicParameters();
            var querySegments = new List<string>();

            foreach (AdviceType type in validTypes)
            {
                parameters.Add(string.Format("Type{0}", type.Value), type.Value, DbType.Int32);
                querySegments.Add(string.Format(SQL_LIST_LAST_BY_TYPE, topCondition, type.Value));
            }

            var query = string.Join(" UNION ", querySegments);

            return Query<Advice>(query, parameters);
        }

        public IEnumerable<int> ListTrendingAdvisedAssets(int? top)
        {
            var query = string.Format(SQL_LIST_TRENDING_ADVISED_ASSETS, top);
            return Query<int>(query);
        }

        public void InsertAdvices(List<Advice> newAdvices)
        {
            if (newAdvices == null || !newAdvices.Any())
                return;

            var insertSql = "";
            foreach (var value in newAdvices)
            {
                insertSql += $"INSERT INTO [Advice] (AssetId, AdvisorId, CreationDate, Type, AssetValue, OperationType, TargetPrice, StopLoss) VALUES ({value.AssetId}, {value.AdvisorId}, " +
                            $"{GetDateTimeSqlFormattedValue(value.CreationDate)}, {value.Type}, {GetDoubleSqlFormattedValue(value.AssetValue)}, {value.OperationType}, " +
                            $"{GetDoubleSqlFormattedValue(value.TargetPrice)}, {GetDoubleSqlFormattedValue(value.StopLoss)});";
            }
            Execute(insertSql, null, 240);
        }
    }
}
