using Auctus.DomainObjects.Advisor;
using DataAccessInterfaces;

namespace Auctus.DataAccessInterfaces.Advisor
{
    public interface IRequestToBeAdvisorData<T> : IBaseData<T>
    {
        RequestToBeAdvisor GetByUser(int userId);
    }
}