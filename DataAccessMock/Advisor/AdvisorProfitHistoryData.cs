using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Advisor
{
    public class AdvisorProfitHistoryData : BaseData<AdvisorProfitHistory>, IAdvisorProfitHistoryData<AdvisorProfitHistory>
    {
        public void SetAdvisorProfitHistory(DateTime referenceDate, IEnumerable<AdvisorProfit> advisorsProfit)
        {
            throw new NotImplementedException();
        }
    }
}
