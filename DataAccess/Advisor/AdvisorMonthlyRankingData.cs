using Auctus.DataAccess.Core;
using Auctus.DomainObjects.Advisor;
using Microsoft.Extensions.Configuration;
using Auctus.DataAccessInterfaces.Advisor;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Data;
using System.Linq;

namespace Auctus.DataAccess.Advisor
{
    public class AdvisorMonthlyRankingData : BaseSql<AdvisorMonthlyRanking>, IAdvisorMonthlyRankingData<AdvisorMonthlyRanking>
    {
        public override string TableName => "AdvisorMonthlyRanking";

        private const string SQL_HALL_OF_FAME = "SELECT a.* FROM [AdvisorMonthlyRanking] a WITH(NOLOCK) WHERE {0} ORDER BY a.Year DESC, a.Month DESC, a.Ranking";

        public AdvisorMonthlyRankingData(IConfigurationRoot configuration) : base(configuration) { }

        public List<AdvisorMonthlyRanking> ListAdvisorMonthlyRanking(int year, int month)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Year", year, DbType.Int32);
            parameters.Add("Month", month, DbType.Int32);
            return SelectByParameters<AdvisorMonthlyRanking>(parameters, "Ranking").ToList();
        }

        public List<AdvisorMonthlyRanking> ListAdvisorsHallOfFame(int topAmount)
        {
            var parameters = new DynamicParameters();
            var keys = new List<string>();
            for (var i = 1; i <= topAmount; i++)
            {
                var key = $"Ranking{i}";
                parameters.Add(key, i, DbType.Int32);
                keys.Add($"a.Ranking = @{key}");
            }
            return Query<AdvisorMonthlyRanking>(string.Format(SQL_HALL_OF_FAME, string.Join(" OR ", keys)), parameters).ToList();
        }

        public void SetAdvisorMonthlyRanking(IEnumerable<AdvisorMonthlyRanking> advisorsMonthlyRanking)
        {
            if (advisorsMonthlyRanking == null || !advisorsMonthlyRanking.Any())
                return;

            var executeSql = "";
            foreach (var advisor in advisorsMonthlyRanking)
                executeSql += GetInsertScript(advisor);

            Execute(executeSql, null, 120);
        }

        private string GetInsertScript(AdvisorMonthlyRanking advisorMonthlyRanking)
        {
            return $"INSERT INTO [AdvisorMonthlyRanking] (UserId, Year, Month, CreationDate, Ranking, AverageReturn) VALUES ({advisorMonthlyRanking.UserId}, {advisorMonthlyRanking.Year}, {advisorMonthlyRanking.Month}, {GetDateTimeSqlFormattedValue(advisorMonthlyRanking.CreationDate)}, {advisorMonthlyRanking.Ranking}, {GetDoubleSqlFormattedValue(advisorMonthlyRanking.AverageReturn)});";
        }
    }
}
