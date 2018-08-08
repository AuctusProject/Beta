using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Advisor
{
    public class AdviceData : BaseData<Advice>, IAdviceData<Advice>
    {
        public List<Advice> List(IEnumerable<int> advisorIds)
        {
            throw new System.NotImplementedException();
        }
    }
}
