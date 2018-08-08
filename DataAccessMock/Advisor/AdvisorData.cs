using Auctus.DataAccessInterfaces.Advisor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Advisor
{
    public class AdvisorData : BaseData<DomainObjects.Advisor.Advisor>, IAdvisorData<DomainObjects.Advisor.Advisor>
    {
        public List<DomainObjects.Advisor.Advisor> ListEnabled()
        {
            throw new NotImplementedException();
        }
    }
}
