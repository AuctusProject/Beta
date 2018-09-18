using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Advisor
{
    public class AdviceData : BaseData<Advice>, IAdviceData<Advice>
    {
        public override void Insert(Advice obj)
        {
            //Intentionally left empty for unit testing
        }

        private List<Advice> GetAllAdvices()
        {
            var advices = new List<Advice>();
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 5, 9, 16, 35, 0), AdviceType.Buy, 9082.646530291));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 5, 12, 4, 15, 0), AdviceType.ClosePosition, 9345.220727269));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 5, 23, 14, 25, 0), AdviceType.Buy, 8234.858416313));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 6, 1, 11, 55, 0), AdviceType.Buy, 8492.000120634));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 6, 3, 10, 25, 0), AdviceType.Buy, 8309.144420821));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 6, 6, 22, 55, 0), AdviceType.Buy, 7617.852268494));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 6, 13, 12, 15, 0), AdviceType.ClosePosition, 7340.490949843));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 6, 18, 15, 0, 0), AdviceType.Sell, 7506.016064447));

            advices.Add(GetAdvice(2, 1, new DateTime(2018, 5, 9, 20, 30, 0), AdviceType.Buy, 9124.667432005));
            advices.Add(GetAdvice(2, 1, new DateTime(2018, 5, 12, 12, 10, 0), AdviceType.Buy, 9127.505521919));
            advices.Add(GetAdvice(2, 1, new DateTime(2018, 5, 21, 9, 50, 0), AdviceType.Sell, 8827.824550585));
            advices.Add(GetAdvice(2, 1, new DateTime(2018, 6, 6, 23, 30, 0), AdviceType.ClosePosition, 7594.508928211));

            advices.Add(GetAdvice(3, 1, new DateTime(2018, 5, 11, 13, 5, 0), AdviceType.Sell, 9376.94929085));
            advices.Add(GetAdvice(3, 1, new DateTime(2018, 5, 18, 13, 55, 0), AdviceType.ClosePosition, 8711.660107644));

            advices.Add(GetAdvice(1, 2, new DateTime(2018, 5, 26, 16, 35, 0), AdviceType.Buy, 675.615056342));
            advices.Add(GetAdvice(1, 2, new DateTime(2018, 5, 31, 4, 15, 0), AdviceType.Buy, 711.613496804));
            advices.Add(GetAdvice(1, 2, new DateTime(2018, 6, 5, 1, 10, 0), AdviceType.ClosePosition, 634.151407759));
            advices.Add(GetAdvice(1, 2, new DateTime(2018, 6, 8, 11, 55, 0), AdviceType.Sell, 588.668853734));
            advices.Add(GetAdvice(1, 2, new DateTime(2018, 6, 17, 2, 25, 0), AdviceType.ClosePosition, 524.975930399));

            advices.Add(GetAdvice(2, 2, new DateTime(2018, 5, 9, 10, 30, 0), AdviceType.Buy, 726.226813111));
            advices.Add(GetAdvice(2, 2, new DateTime(2018, 5, 13, 17, 10, 0), AdviceType.Buy, 691.427017153));
            advices.Add(GetAdvice(2, 2, new DateTime(2018, 5, 22, 1, 40, 0), AdviceType.Sell, 715.108112595));

            advices.Add(GetAdvice(2, 3, new DateTime(2018, 5, 9, 16, 35, 0), AdviceType.Sell, 0.771565267));
            advices.Add(GetAdvice(2, 3, new DateTime(2018, 5, 16, 4, 15, 0), AdviceType.Sell, 0.677504694));
            advices.Add(GetAdvice(2, 3, new DateTime(2018, 5, 21, 4, 30, 0), AdviceType.Sell, 0.748056592));
            advices.Add(GetAdvice(2, 3, new DateTime(2018, 6, 16, 19, 55, 0), AdviceType.Sell, 0.556373826));
            advices.Add(GetAdvice(2, 3, new DateTime(2018, 6, 27, 22, 55, 0), AdviceType.Sell, 0.698993864));

            advices.Add(GetAdvice(2, 4, new DateTime(2018, 5, 15, 4, 15, 0), AdviceType.Buy, 0.70635167));
            advices.Add(GetAdvice(2, 4, new DateTime(2018, 6, 16, 17, 20, 0), AdviceType.ClosePosition, 0.327003675));
            return advices.OrderBy(c => c.CreationDate).Select((c, i) =>
            {
                c.Id = i + 1;
                return c;
            }).ToList();
        }

        public List<Advice> List(IEnumerable<int> advisorIds = null, IEnumerable<int> assetsIds = null)
        {
            List<Advice> advices = GetAllAdvices();

            return advisorIds != null && assetsIds == null ? advices.Where(a => advisorIds.Contains(a.AdvisorId)).ToList() :
                assetsIds != null && advisorIds != null ? advices.Where(a => advisorIds.Contains(a.AdvisorId) && assetsIds.Contains(a.AssetId)).ToList() :
                assetsIds != null && advisorIds == null ? advices.Where(a => assetsIds.Contains(a.AssetId)).ToList() : advices;
        }

        
        private Advice GetAdvice(int userId, int assetId, DateTime dateTime, AdviceType type, double assetValue)
        {
            return new Advice()
            {
                AssetId = assetId,
                CreationDate = dateTime,
                Type = type.Value,
                AdvisorId = userId,
                AssetValue = assetValue
            };
        }

        public Advice GetLastAdviceForAssetByAdvisor(int assetId, int advisorId)
        {
            switch (assetId)
            {
                case 1:
                    return GetAdvice(advisorId, 1, new DateTime(2018, 5, 9, 16, 35, 0), AdviceType.Buy, 9082.646530291);
                case 2:
                    return GetAdvice(advisorId, 2, GetDateTimeNow(), AdviceType.Sell, 595.28951907);
                case 3:
                    return GetAdvice(advisorId, 3, new DateTime(2018, 5, 9, 16, 35, 0), AdviceType.ClosePosition, 0.771565267);
                default: return null;
            }
        }

        public IEnumerable<Advice> ListLastAdvicesWithPagination(IEnumerable<int> advisorsIds, IEnumerable<int> assetsIds, int? top, int? lastAdviceId)
        {
            IEnumerable<Advice> advices = GetAllAdvices().Where(a => advisorsIds.Contains(a.AdvisorId) || assetsIds.Contains(a.AssetId));
            if(lastAdviceId.HasValue)
                advices = advices.Where(a => a.Id < lastAdviceId.Value);

            advices = advices.OrderByDescending(a => a.CreationDate);

            if (top.HasValue)
                advices = advices.Take(top.Value);

            return advices;
        }

        public IEnumerable<Advice> ListLastAdvicesForAllTypes(int? top)
        {
            var result = new List<Advice>();
            var allAdvices = GetAllAdvices();

            result.AddRange(allAdvices.Where(advice => advice.AdviceType == AdviceType.Sell).OrderByDescending(advice => advice.CreationDate).Take(top ?? 3));
            result.AddRange(allAdvices.Where(advice => advice.AdviceType == AdviceType.Buy).OrderByDescending(advice => advice.CreationDate).Take(top ?? 3));
            result.AddRange(allAdvices.Where(advice => advice.AdviceType == AdviceType.ClosePosition).OrderByDescending(advice => advice.CreationDate).Take(top ?? 3));

            return result;
        }

        public IEnumerable<int> ListTrendingAdvisedAssets(int? top)
        {
            return new int[] { 1, 2, 3 };
        }
    }
}
