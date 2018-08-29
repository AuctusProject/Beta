using System.Collections.Generic;
using Auctus.DomainObjects.Advisor;

namespace Auctus.DataAccessInterfaces.Advisor
{
    public interface IAdvisorData<T> : IBaseData<T>
    {
        List<DomainObjects.Advisor.Advisor> ListEnabled();
        IEnumerable<DomainObjects.Advisor.Advisor> ListFollowingAdvisors(int userId);
        DomainObjects.Advisor.Advisor GetAdvisor(int id);
    }
}