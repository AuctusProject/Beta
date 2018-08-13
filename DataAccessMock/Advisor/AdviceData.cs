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
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 5, 9, 16, 35, 0), AdviceType.Buy));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 5, 12, 4, 15, 0), AdviceType.ClosePosition));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 5, 23, 14, 25, 0), AdviceType.Buy));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 6, 1, 11, 55, 0), AdviceType.Buy));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 6, 3, 10, 25, 0), AdviceType.Buy));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 6, 6, 22, 55, 0), AdviceType.Buy));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 6, 13, 12, 15, 0), AdviceType.ClosePosition));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 6, 18, 15, 0, 0), AdviceType.Sell));

            advices.Add(GetAdvice(2, 1, new DateTime(2018, 5, 9, 20, 30, 0), AdviceType.Buy));
            advices.Add(GetAdvice(2, 1, new DateTime(2018, 5, 12, 12, 10, 0), AdviceType.Buy));
            advices.Add(GetAdvice(2, 1, new DateTime(2018, 5, 21, 9, 50, 0), AdviceType.Sell));
            advices.Add(GetAdvice(2, 1, new DateTime(2018, 6, 6, 23, 30, 0), AdviceType.ClosePosition));

            advices.Add(GetAdvice(3, 1, new DateTime(2018, 5, 11, 13, 5, 0), AdviceType.Sell));
            advices.Add(GetAdvice(3, 1, new DateTime(2018, 5, 18, 13, 55, 0), AdviceType.ClosePosition));

            advices.Add(GetAdvice(1, 2, new DateTime(2018, 5, 26, 16, 35, 0), AdviceType.Buy));
            advices.Add(GetAdvice(1, 2, new DateTime(2018, 5, 31, 4, 15, 0), AdviceType.Buy));
            advices.Add(GetAdvice(1, 2, new DateTime(2018, 6, 5, 1, 10, 0), AdviceType.ClosePosition));
            advices.Add(GetAdvice(1, 2, new DateTime(2018, 6, 8, 11, 55, 0), AdviceType.Sell));
            advices.Add(GetAdvice(1, 2, new DateTime(2018, 6, 17, 2, 25, 0), AdviceType.ClosePosition));

            advices.Add(GetAdvice(2, 2, new DateTime(2018, 5, 9, 10, 30, 0), AdviceType.Buy));
            advices.Add(GetAdvice(2, 2, new DateTime(2018, 5, 13, 17, 10, 0), AdviceType.Buy));
            advices.Add(GetAdvice(2, 2, new DateTime(2018, 5, 22, 1, 40, 0), AdviceType.Sell));

            advices.Add(GetAdvice(2, 3, new DateTime(2018, 5, 9, 16, 35, 0), AdviceType.Sell));
            advices.Add(GetAdvice(2, 3, new DateTime(2018, 5, 16, 4, 15, 0), AdviceType.Sell));
            advices.Add(GetAdvice(2, 3, new DateTime(2018, 5, 21, 4, 30, 0), AdviceType.Sell));
            advices.Add(GetAdvice(2, 3, new DateTime(2018, 6, 16, 19, 55, 0), AdviceType.Sell));
            advices.Add(GetAdvice(2, 3, new DateTime(2018, 6, 27, 22, 55, 0), AdviceType.Sell));

            advices.Add(GetAdvice(2, 4, new DateTime(2018, 5, 15, 4, 15, 0), AdviceType.Buy));
            advices.Add(GetAdvice(2, 4, new DateTime(2018, 6, 16, 17, 20, 0), AdviceType.ClosePosition));
            return advices.OrderBy(c => c.CreationDate).Select((c, i) =>
            {
                c.Id = i + 1;
                return c;
            }).ToList();
        }

        public List<Advice> List(IEnumerable<int> advisorIds)
        {
            List<Advice> advices = GetAllAdvices();

            return advices.Where(a => advisorIds.Contains(a.AdvisorId)).ToList();
        }

        
        private Advice GetAdvice(int userId, int assetId, DateTime dateTime, AdviceType type)
        {
            return new Advice()
            {
                AssetId = assetId,
                CreationDate = dateTime,
                Type = type.Value,
                AdvisorId = userId
            };
        }

        public Advice GetLastAdviceForAssetByAdvisor(int assetId, int advisorId)
        {
            switch (assetId)
            {
                case 1:
                    return GetAdvice(advisorId, 1, new DateTime(2018, 5, 9, 16, 35, 0), AdviceType.Buy);
                case 2:
                    return GetAdvice(advisorId, 2, GetDateTimeNow(), AdviceType.Sell);
                case 3:
                    return GetAdvice(advisorId, 3, new DateTime(2018, 5, 9, 16, 35, 0), AdviceType.ClosePosition);
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
    }
}
