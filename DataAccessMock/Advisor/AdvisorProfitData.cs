using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using Auctus.DomainObjects.Trade;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Auctus.DataAccessMock.Advisor
{
    public class AdvisorProfitData : BaseData<AdvisorProfit>, IAdvisorProfitData<AdvisorProfit>
    {
        static List<AdvisorProfit> advisorProfit = new List<AdvisorProfit>()
        {
            new AdvisorProfit()
            {
                AssetId = 241,
                OrderCount = 1,
                Status = OrderStatusType.Executed.Value,
                Type = OrderType.Buy.Value,
                SuccessCount = 0,
                SummedProfitDollar = 0,
                SummedProfitPercentage = 0,
                TotalDollar = 100000,
                TotalQuantity = 100000,
                UpdateDate = new DateTime(2017,1,1),
                UserId = 1
            }
        };

        public IEnumerable<AdvisorProfit> ListAdvisorProfit(int advisorId, IEnumerable<int> assetIds)
        {
            return advisorProfit.Where(a => a.UserId == advisorId && (assetIds == null || !assetIds.Any() || assetIds.Contains(a.AssetId))).ToList();
        }

        public void SetAdvisorProfit(IEnumerable<AdvisorProfit> advisorsProfit)
        {
            throw new NotImplementedException();
        }

        public override void Delete(AdvisorProfit obj)
        {
            advisorProfit.Remove(obj);
        }

        public override void Insert(AdvisorProfit obj)
        {
            advisorProfit.Add(obj);
        }

        public override void Update(AdvisorProfit obj)
        {
            //Do nothing
        }

        public List<AdvisorProfit> ListAdvisorProfit(IEnumerable<int> advisorIds, IEnumerable<int> assetIds)
        {
            return advisorProfit.Where(a => (advisorIds == null || !advisorIds.Any() || advisorIds.Contains(a.UserId)) &&
                (assetIds == null || !assetIds.Any() || assetIds.Contains(a.AssetId))).ToList();
        }
    }
}
