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
    public class AdvisorRankingHistoryData : BaseSql<AdvisorRankingHistory>, IAdvisorRankingHistoryData<AdvisorRankingHistory>
    {
        private const string SQL_LIST = @"SELECT * FROM 
                                            [AdvisorRankingHistory] r WITH(NOLOCK)
                                            LEFT JOIN [AdvisorProfitHistory] p WITH(NOLOCK) ON p.UserId = r.UserId AND p.ReferenceDate = r.ReferenceDate
                                            WHERE r.ReferenceDate >= @StartDate AND r.ReferenceDate < @EndDate AND {0} ORDER BY r.ReferenceDate";

        public override string TableName => "AdvisorRankingHistory";
        public AdvisorRankingHistoryData(IConfigurationRoot configuration) : base(configuration) { }

        public List<AdvisorRankingHistory> ListAdvisorRankingAndProfitHistory(IEnumerable<int> advisorsId, DateTime startDate, DateTime endDate)
        {
            if (!(advisorsId?.Any() == true))
                throw new ArgumentNullException("advisorsId");

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("StartDate", startDate, DbType.DateTime);
            parameters.Add("EndDate", endDate, DbType.DateTime);
            var complement = $"({string.Join(" OR ", advisorsId.Select((c, i) => $"r.UserId = @UserId{i}"))})";
            for (int i = 0; i < advisorsId.Count(); ++i)
                parameters.Add($"UserId{i}", advisorsId.ElementAt(i), DbType.Int32);

            Dictionary<int, Dictionary<DateTime, AdvisorRankingHistory>> cache = new Dictionary<int, Dictionary<DateTime, AdvisorRankingHistory>>();
            return Query<AdvisorRankingHistory, AdvisorProfitHistory, AdvisorRankingHistory>(string.Format(SQL_LIST, complement), 
                (history, profit) =>
                {
                    if (!cache.ContainsKey(history.UserId))
                        cache[history.UserId] = new Dictionary<DateTime, AdvisorRankingHistory>();
                    if (!cache[history.UserId].ContainsKey(history.ReferenceDate))
                        cache[history.UserId].Add(history.ReferenceDate, history);

                    if (profit != null)
                        cache[history.UserId][history.ReferenceDate].AdvisorProfitHistory.Add(profit);

                    return history;
                }, "UserId", parameters).ToList();
        }

        public void SetAdvisorRankingHistory(DateTime referenceDate, IEnumerable<AdvisorRanking> advisorsRanking)
        {
            if (advisorsRanking == null || !advisorsRanking.Any())
                return;

            var executeSql = "";
            foreach (var advisorRanking in advisorsRanking)
                executeSql += GetInsertScript(referenceDate, advisorRanking);
            
            Execute(executeSql, null, 120);
        }

        private string GetInsertScript(DateTime referenceDate, AdvisorRanking advisorRanking)
        {
            return $"INSERT INTO [AdvisorRankingHistory] (UserId, ReferenceDate, Ranking, Rating) VALUES ({advisorRanking.Id}, {GetDateTimeSqlFormattedValue(referenceDate)}, {advisorRanking.Ranking}, {GetDoubleSqlFormattedValue(advisorRanking.Rating)});";
        }
    }
}
