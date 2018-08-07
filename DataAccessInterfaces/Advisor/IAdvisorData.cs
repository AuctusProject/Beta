using System.Collections.Generic;

namespace Auctus.DataAccessInterfaces.Advisor
{
    public interface IAdvisorData<T> : IBaseData<T>
    {
        List<DomainObjects.Advisor.Advisor> ListEnabled();
    }
}