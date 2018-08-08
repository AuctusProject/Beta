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
        public List<Advice> List(IEnumerable<int> advisorIds)
        {
            var advices = new List<Advice>();
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 8, 9, 16, 35, 0), AdviceType.Buy));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 8, 12, 4, 15, 0), AdviceType.ClosePosition));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 8, 23, 14, 25, 0), AdviceType.Buy));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 9, 1, 11, 55, 0), AdviceType.Buy));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 9, 3, 10, 25, 0), AdviceType.Buy));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 9, 6, 22, 55, 0), AdviceType.Buy));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 9, 13, 12, 15, 0), AdviceType.ClosePosition));
            advices.Add(GetAdvice(1, 1, new DateTime(2018, 9, 18, 15, 0, 0), AdviceType.Sell));

            advices.Add(GetAdvice(2, 1, new DateTime(2018, 8, 9, 20, 30, 0), AdviceType.Buy));
            advices.Add(GetAdvice(2, 1, new DateTime(2018, 8, 12, 12, 10, 0), AdviceType.Buy));
            advices.Add(GetAdvice(2, 1, new DateTime(2018, 8, 21, 9, 50, 0), AdviceType.Sell));
            advices.Add(GetAdvice(2, 1, new DateTime(2018, 9, 6, 23, 30, 0), AdviceType.ClosePosition));

            advices.Add(GetAdvice(3, 1, new DateTime(2018, 8, 11, 13, 5, 0), AdviceType.Sell));
            advices.Add(GetAdvice(3, 1, new DateTime(2018, 8, 18, 13, 55, 0), AdviceType.ClosePosition));

            advices.Add(GetAdvice(1, 2, new DateTime(2018, 8, 26, 16, 35, 0), AdviceType.Buy));
            advices.Add(GetAdvice(1, 2, new DateTime(2018, 8, 31, 4, 15, 0), AdviceType.Buy));
            advices.Add(GetAdvice(1, 2, new DateTime(2018, 9, 5, 1, 10, 0), AdviceType.ClosePosition));
            advices.Add(GetAdvice(1, 2, new DateTime(2018, 9, 8, 11, 55, 0), AdviceType.Sell));
            advices.Add(GetAdvice(1, 2, new DateTime(2018, 9, 17, 2, 25, 0), AdviceType.ClosePosition));

            advices.Add(GetAdvice(2, 2, new DateTime(2018, 8, 9, 10, 30, 0), AdviceType.Buy));
            advices.Add(GetAdvice(2, 2, new DateTime(2018, 8, 13, 17, 10, 0), AdviceType.Buy));
            advices.Add(GetAdvice(2, 2, new DateTime(2018, 8, 22, 1, 40, 0), AdviceType.Sell));

            advices.Add(GetAdvice(2, 3, new DateTime(2018, 8, 9, 16, 35, 0), AdviceType.Sell));
            advices.Add(GetAdvice(2, 3, new DateTime(2018, 8, 16, 4, 15, 0), AdviceType.Sell));
            advices.Add(GetAdvice(2, 3, new DateTime(2018, 8, 21, 4, 30, 0), AdviceType.Sell));
            advices.Add(GetAdvice(2, 3, new DateTime(2018, 9, 16, 19, 55, 0), AdviceType.Sell));
            advices.Add(GetAdvice(2, 3, new DateTime(2018, 9, 27, 22, 55, 0), AdviceType.Sell));

            advices.Add(GetAdvice(2, 4, new DateTime(2018, 8, 15, 4, 15, 0), AdviceType.Buy));
            advices.Add(GetAdvice(2, 4, new DateTime(2018, 9, 16, 17, 20, 0), AdviceType.ClosePosition));

            return advices.OrderBy(c => c.CreationDate).Select((c, i) =>
            {
                c.Id = i + 1;
                return c;
            }).ToList();
        }

        private Advice GetAdvice(int userId, int assetId, DateTime dateTime, AdviceType type)
        {
            return new Advice()
            {
                AssetId = assetId,
                CreationDate = dateTime,
                Type = type.Value,
                UserId = userId
            };
        }
    }
}
