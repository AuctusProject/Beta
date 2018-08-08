using Auctus.DomainObjects.Advisor;
using System.Collections.Generic;

namespace Auctus.DataAccessInterfaces.Advisor
{
    public interface IAdviceData<T> : IBaseData<T>
    {
        List<Advice> List(IEnumerable<int> advisorIds);
    }
}