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
using System.Threading.Tasks;

namespace Auctus.DataAccess.Advisor
{
    public class AdvisorProfitData : BaseSql<AdvisorProfit>, IAdvisorProfitData<AdvisorProfit>
    {
        public override string TableName => "AdvisorProfit";

        private const string SQL_LIST = @"SELECT ap.* FROM [AdvisorProfit] ap WITH(NOLOCK) {0}";

        public AdvisorProfitData(IConfigurationRoot configuration) : base(configuration) { }

        public void SetAdvisorProfit(IEnumerable<AdvisorProfit> advisorsProfit)
        {
            if (advisorsProfit == null || !advisorsProfit.Any())
                return;

            var currentAdvisorsProfit = SelectByParameters<AdvisorProfit>(null);
            var sqlUserAssetBlock = new Dictionary<int, Dictionary<int, string>>();
            foreach (var advisorProfit in advisorsProfit)
            {
                if (!sqlUserAssetBlock.ContainsKey(advisorProfit.UserId))
                    sqlUserAssetBlock[advisorProfit.UserId] = new Dictionary<int, string>();
                if (!sqlUserAssetBlock[advisorProfit.UserId].ContainsKey(advisorProfit.AssetId))
                    sqlUserAssetBlock[advisorProfit.UserId][advisorProfit.AssetId] = "";

                var existing = currentAdvisorsProfit.FirstOrDefault(c => c.UserId == advisorProfit.UserId && c.AssetId == advisorProfit.AssetId && c.Status == advisorProfit.Status && c.Type == advisorProfit.Type);
                if (existing == null)
                    sqlUserAssetBlock[advisorProfit.UserId][advisorProfit.AssetId] += GetInsertScript(advisorProfit);
                else if (existing.UpdateDate < advisorProfit.UpdateDate)
                    sqlUserAssetBlock[advisorProfit.UserId][advisorProfit.AssetId] += GetUpdateScript(advisorProfit, existing);
            }
            var excludedItens = currentAdvisorsProfit.Where(c => !advisorsProfit.Any(a => c.UserId == a.UserId && c.AssetId == a.AssetId && c.Status == a.Status && c.Type == a.Type));
            foreach (var excluded in excludedItens)
            {
                if (!sqlUserAssetBlock.ContainsKey(excluded.UserId))
                    sqlUserAssetBlock[excluded.UserId] = new Dictionary<int, string>();
                if (!sqlUserAssetBlock[excluded.UserId].ContainsKey(excluded.AssetId))
                    sqlUserAssetBlock[excluded.UserId][excluded.AssetId] = "";

                sqlUserAssetBlock[excluded.UserId][excluded.AssetId] += GetDeleteScript(excluded);
            }

            Email.EmailResource emailResource = null;
            Parallel.ForEach(sqlUserAssetBlock, new ParallelOptions() { MaxDegreeOfParallelism = 5 }, c =>
            {
                foreach (var script in c.Value)
                {
                    if (!string.IsNullOrEmpty(script.Value))
                    {
                        try
                        {
                            Execute(script.Value);
                        }
                        catch (Exception ex)
                        {
                            if (emailResource == null)
                                emailResource = new Email.EmailResource(Configuration, false);
                            emailResource.SendAsync(emailResource.EmailErrorList, $"Error on SetAdvisorProfit - UserId {c.Key} - AssetId {script.Key}", ex.ToString()).Wait();
                        }
                    }
                }
            });
        }

        private string GetInsertScript(AdvisorProfit advisorProfit)
        {
            var baseInsert = "INSERT INTO [AdvisorProfit] (UserId, AssetId, Status, Type, UpdateDate, SummedProfitPercentage, SummedProfitDollar, TotalDollar, TotalQuantity, OrderCount, SuccessCount, SummedTradeMinutes, TotalFee) VALUES ({0});";
            return string.Format(baseInsert, $"{advisorProfit.UserId}, {advisorProfit.AssetId}, {advisorProfit.Status}, {advisorProfit.Type}, {GetDateTimeSqlFormattedValue(advisorProfit.UpdateDate)}, {GetDoubleSqlFormattedValue(advisorProfit.SummedProfitPercentage)}, {GetDoubleSqlFormattedValue(advisorProfit.SummedProfitDollar)}, {GetDoubleSqlFormattedValue(advisorProfit.TotalDollar)}, {GetDoubleSqlFormattedValue(advisorProfit.TotalQuantity)}, {advisorProfit.OrderCount}, {advisorProfit.SuccessCount}, {GetNullableValue(advisorProfit.SummedTradeMinutes)}, {GetDoubleSqlFormattedValue(advisorProfit.TotalFee)}");
        }

        private string GetDeleteScript(AdvisorProfit advisorProfit)
        {
            return $"DELETE FROM [AdvisorProfit] WHERE UserId = {advisorProfit.UserId} AND AssetId = {advisorProfit.AssetId} AND Status = {advisorProfit.Status} AND Type = {advisorProfit.Type} AND UpdateDate < {GetDateTimeSqlFormattedValue(advisorProfit.UpdateDate)};";
        }

        private string GetUpdateScript(AdvisorProfit newAdvisorProfit, AdvisorProfit oldAdvisorProfit)
        {
            var update = new List<string>();
            if (!newAdvisorProfit.SummedProfitPercentage.Equals6DigitPrecision(oldAdvisorProfit.SummedProfitPercentage))
                update.Add($"SummedProfitPercentage = {GetDoubleSqlFormattedValue(newAdvisorProfit.SummedProfitPercentage)}");
            if (!newAdvisorProfit.SummedProfitDollar.Equals6DigitPrecision(oldAdvisorProfit.SummedProfitDollar))
                update.Add($"SummedProfitDollar = {GetDoubleSqlFormattedValue(newAdvisorProfit.SummedProfitDollar)}");
            if (!newAdvisorProfit.TotalDollar.Equals6DigitPrecision(oldAdvisorProfit.TotalDollar))
                update.Add($"TotalDollar = {GetDoubleSqlFormattedValue(newAdvisorProfit.TotalDollar)}");
            if (!newAdvisorProfit.TotalQuantity.Equals8DigitPrecision(oldAdvisorProfit.TotalQuantity))
                update.Add($"TotalQuantity = {GetDoubleSqlFormattedValue(newAdvisorProfit.TotalQuantity)}");
            if (newAdvisorProfit.OrderCount != oldAdvisorProfit.OrderCount)
                update.Add($"OrderCount = {newAdvisorProfit.OrderCount}");
            if (newAdvisorProfit.SuccessCount != oldAdvisorProfit.SuccessCount)
                update.Add($"SuccessCount = {newAdvisorProfit.SuccessCount}");
            if (newAdvisorProfit.SummedTradeMinutes != oldAdvisorProfit.SummedTradeMinutes)
                update.Add($"SummedTradeMinutes = {GetNullableValue(newAdvisorProfit.SummedTradeMinutes)}");
            if ((newAdvisorProfit.TotalFee.HasValue && !oldAdvisorProfit.TotalFee.HasValue) || (!newAdvisorProfit.TotalFee.HasValue && oldAdvisorProfit.TotalFee.HasValue)
                || (newAdvisorProfit.TotalFee.HasValue && oldAdvisorProfit.TotalFee.HasValue && !newAdvisorProfit.TotalFee.Value.Equals6DigitPrecision(oldAdvisorProfit.TotalFee.Value)))
                update.Add($"TotalFee = {GetDoubleSqlFormattedValue(newAdvisorProfit.TotalFee)}");

            return update.Count == 0 ? "" : $"UPDATE [AdvisorProfit] SET UpdateDate = {GetDateTimeSqlFormattedValue(newAdvisorProfit.UpdateDate)},{string.Join(',', update)} WHERE UserId = {newAdvisorProfit.UserId} AND AssetId = {newAdvisorProfit.AssetId} AND Status = {newAdvisorProfit.Status} AND Type = {newAdvisorProfit.Type} AND UpdateDate < {GetDateTimeSqlFormattedValue(newAdvisorProfit.UpdateDate)};";
        }

        public List<AdvisorProfit> ListAdvisorProfit(IEnumerable<int> advisorIds, IEnumerable<int> assetIds)
        {
            var parameters = new DynamicParameters();
            var complement = advisorIds?.Any() == true || assetIds?.Any() == true ? " WHERE " : "";
            if (advisorIds?.Any() == true)
            {
                complement += $"({string.Join(" OR ", advisorIds.Select((c, i) => $"ap.UserId = @UserId{i}"))})";
                for (int i = 0; i < advisorIds.Count(); ++i)
                    parameters.Add($"UserId{i}", advisorIds.ElementAt(i), DbType.Int32);
            }
            if (assetIds?.Any() == true)
            {
                if (advisorIds?.Any() == true)
                    complement += " AND ";

                complement += $"({string.Join(" OR ", assetIds.Select((c, i) => $"ap.AssetId = @AssetId{i}"))})";
                for (int i = 0; i < assetIds.Count(); ++i)
                    parameters.Add($"AssetId{i}", assetIds.ElementAt(i), DbType.Int32);
            }
            return Query<AdvisorProfit>(string.Format(SQL_LIST, complement), parameters).ToList();
        }
    }
}
