using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Advisor
{
    public class AdvisorMonthlyRankingData : BaseData<AdvisorMonthlyRanking>, IAdvisorMonthlyRankingData<AdvisorMonthlyRanking>
    {
        public List<AdvisorMonthlyRanking> ListAdvisorMonthlyRanking(int year, int month)
        {
            throw new NotImplementedException();
        }

        public List<AdvisorMonthlyRanking> ListAdvisorsHallOfFame(int topAmount)
        {
            throw new NotImplementedException();
        }

        public void SetAdvisorMonthlyRanking(IEnumerable<AdvisorMonthlyRanking> advisorsMonthlyRanking)
        {
            throw new NotImplementedException();
        }
    }
}
