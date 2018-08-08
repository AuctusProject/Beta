using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Advisor
{
    public class RequestToBeAdvisorData : BaseData<RequestToBeAdvisor>, IRequestToBeAdvisorData<RequestToBeAdvisor>
    {
        public RequestToBeAdvisor GetByUser(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
