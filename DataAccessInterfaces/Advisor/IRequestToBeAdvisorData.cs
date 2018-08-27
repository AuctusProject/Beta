using System.Collections.Generic;
using Auctus.DomainObjects.Advisor;

namespace Auctus.DataAccessInterfaces.Advisor
{
    public interface IRequestToBeAdvisorData<T> : IBaseData<T>
    {
        RequestToBeAdvisor GetByUser(int userId);
        List<RequestToBeAdvisor> ListPending();
    }
}