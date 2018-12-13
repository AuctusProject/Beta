using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Auctus.Util;

namespace Auctus.DataAccess.Advisor
{
    public class AdvisorRankingData : BaseSql<AdvisorRanking>, IAdvisorRankingData<AdvisorRanking>
    {
        private const string SQL_LIST_WITH_PROFIT = @"SELECT * FROM 
                                                    [User] u WITH(NOLOCK)
                                                    INNER JOIN [Advisor] a WITH(NOLOCK) ON a.Id = u.Id
                                                    INNER JOIN [AdvisorRanking] r WITH(NOLOCK) ON r.Id = a.Id 
                                                    INNER JOIN [AdvisorProfit] p WITH(NOLOCK) ON p.UserId = a.Id 
                                                    {0}";

        private const string SQL_LIST = @"SELECT * FROM 
                                            [User] u WITH(NOLOCK)
                                            INNER JOIN [Advisor] a WITH(NOLOCK) ON a.Id = u.Id
                                            INNER JOIN [AdvisorRanking] r WITH(NOLOCK) ON r.Id = a.Id 
                                            {0}";

        public override string TableName => "AdvisorRanking";
        public AdvisorRankingData(IConfigurationRoot configuration) : base(configuration) { }

        public List<AdvisorRanking> ListAdvisorsRankingAndProfit(IEnumerable<int> advisorsId, IEnumerable<int> assetsId)
        {
            DynamicParameters parameters = new DynamicParameters();
            var complement = advisorsId?.Any() == true || assetsId?.Any() == true ? " WHERE " : "";

            if (advisorsId?.Any() == true)
            {
                complement += $"({string.Join(" OR ", advisorsId.Select((c, i) => $"u.Id = @Id{i}"))})";
                for (int i = 0; i < advisorsId.Count(); ++i)
                    parameters.Add($"Id{i}", advisorsId.ElementAt(i), DbType.Int32);
            }
            if (assetsId?.Any() == true)
            {
                if (advisorsId?.Any() == true)
                    complement += " AND ";

                complement += $"({string.Join(" OR ", assetsId.Select((c, i) => $"p.AssetId = @AssetId{i}"))})";
                for (int i = 0; i < assetsId.Count(); ++i)
                    parameters.Add($"AssetId{i}", assetsId.ElementAt(i), DbType.Int32);
            }
            return QueryParentChild<AdvisorRanking, AdvisorProfit, int>(string.Format(SQL_LIST_WITH_PROFIT, complement), c => c.Id, c => c.AdvisorProfit, "UserId", parameters).ToList();
        }

        public void SetAdvisorRanking(IEnumerable<AdvisorRanking> advisorsRanking)
        {
            if (advisorsRanking == null || !advisorsRanking.Any())
                return;

            var executeSql = "";
            var currentAdvisorsRanking = SelectByParameters<AdvisorRanking>(null);
            foreach (var advisorRanking in advisorsRanking)
            {
                var existing = currentAdvisorsRanking.FirstOrDefault(c => c.Id == advisorRanking.Id);
                if (existing == null)
                    executeSql += GetInsertScript(advisorRanking);
                else if (existing.UpdateDate < advisorRanking.UpdateDate)
                    executeSql += GetUpdateScript(advisorRanking, existing);
            }
            var excludedItens = currentAdvisorsRanking.Where(c => !advisorsRanking.Any(a => c.Id == a.Id));
            foreach (var excluded in excludedItens)
                executeSql += GetDeleteScript(excluded);

            if (!string.IsNullOrEmpty(executeSql))
                Execute(executeSql, null, 120);
        }

        private string GetInsertScript(AdvisorRanking advisorRanking)
        {
            return $"INSERT INTO [AdvisorRanking] (Id, UpdateDate, Ranking, Rating) VALUES ({advisorRanking.Id}, {GetDateTimeSqlFormattedValue(advisorRanking.UpdateDate)}, {advisorRanking.Ranking}, {GetDoubleSqlFormattedValue(advisorRanking.Rating)});";
        }

        private string GetDeleteScript(AdvisorRanking advisorRanking)
        {
            return $"DELETE FROM [AdvisorRanking] WHERE Id = {advisorRanking.Id};";
        }

        private string GetUpdateScript(AdvisorRanking newAdvisorRanking, AdvisorRanking oldAdvisorRanking)
        {
            var update = new List<string>();
            if (newAdvisorRanking.Ranking != oldAdvisorRanking.Ranking)
                update.Add($"Ranking = {newAdvisorRanking.Ranking}");
            if (!newAdvisorRanking.Rating.Equals6DigitPrecision(oldAdvisorRanking.Rating))
                update.Add($"Rating = {GetDoubleSqlFormattedValue(newAdvisorRanking.Rating)}");

            return update.Count == 0 ? "" : $"UPDATE [AdvisorRanking] SET UpdateDate = {GetDateTimeSqlFormattedValue(newAdvisorRanking.UpdateDate)},{string.Join(',', update)} WHERE Id = {newAdvisorRanking.Id};";
        }

        public List<AdvisorRanking> ListAdvisorsRanking(IEnumerable<int> advisorsId)
        {
            DynamicParameters parameters = new DynamicParameters();
            var complement = advisorsId?.Any() == true ? " WHERE " : "";

            if (advisorsId?.Any() == true)
            {
                complement += $"({string.Join(" OR ", advisorsId.Select((c, i) => $"u.Id = @Id{i}"))})";
                for (int i = 0; i < advisorsId.Count(); ++i)
                    parameters.Add($"Id{i}", advisorsId.ElementAt(i), DbType.Int32);
            }
            return Query<AdvisorRanking>(string.Format(SQL_LIST, complement), parameters).ToList();
        }
    }
}
