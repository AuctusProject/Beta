using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Advisor
{
    public class AdvisorProfitHistoryData : BaseSql<AdvisorProfitHistory>, IAdvisorProfitHistoryData<AdvisorProfitHistory>
    {
        public override string TableName => "AdvisorProfitHistory";
        public AdvisorProfitHistoryData(IConfigurationRoot configuration) : base(configuration) { }

        public void SetAdvisorProfitHistory(DateTime referenceDate, IEnumerable<AdvisorProfit> advisorsProfit)
        {
            if (advisorsProfit == null || !advisorsProfit.Any())
                return;

            var executeSql = "";
            foreach (var advisorProfit in advisorsProfit)
                executeSql += GetInsertScript(referenceDate, advisorProfit);
            
            Execute(executeSql, null, 120);
        }

        private string GetInsertScript(DateTime referenceDate, AdvisorProfit advisorProfit)
        {
            var baseInsert = "INSERT INTO [AdvisorProfitHistory] (UserId, AssetId, Status, Type, ReferenceDate, SummedProfitPercentage, SummedProfitDollar, TotalDollar, TotalQuantity, OrderCount, SuccessCount, SummedTradeMinutes, TotalFee) VALUES ({0});";
            return string.Format(baseInsert, $"{advisorProfit.UserId}, {advisorProfit.AssetId}, {advisorProfit.Status}, {advisorProfit.Type}, {GetDateTimeSqlFormattedValue(referenceDate)}, {GetDoubleSqlFormattedValue(advisorProfit.SummedProfitPercentage)}, {GetDoubleSqlFormattedValue(advisorProfit.SummedProfitDollar)}, {GetDoubleSqlFormattedValue(advisorProfit.TotalDollar)}, {GetDoubleSqlFormattedValue(advisorProfit.TotalQuantity)}, {advisorProfit.OrderCount}, {advisorProfit.SuccessCount}, {GetNullableValue(advisorProfit.SummedTradeMinutes)}, {GetDoubleSqlFormattedValue(advisorProfit.TotalFee)}");
        }
    }
}
