using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Advisor
{
    public class AdviceData : BaseData<Advice>, IAdviceData<Advice>
    {
        public List<Advice> List(IEnumerable<int> advisorIds)
        {
            var id = 0;
            var advices = new List<Advice>();
            advices.Add(GetAdvice(ref id, 1, 1, DateTime.UtcNow, AdviceType.Buy));

            return advices;
        }

        private Advice GetAdvice(ref int id, int userId, int assetId, DateTime dateTime, AdviceType type)
        {
            ++id;
            return new Advice()
            {
                Id = id,
                AssetId = assetId,
                CreationDate = dateTime,
                Type = type.Value,
                UserId = userId
            };
        }
    }
}
