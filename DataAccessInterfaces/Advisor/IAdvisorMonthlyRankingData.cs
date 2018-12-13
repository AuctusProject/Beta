using Auctus.DomainObjects.Advisor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessInterfaces.Advisor
{
    public interface IAdvisorMonthlyRankingData<AdvisorMonthlyRanking> : IBaseData<AdvisorMonthlyRanking>
    {
        List<AdvisorMonthlyRanking> ListAdvisorMonthlyRanking(int year, int month);
        void SetAdvisorMonthlyRanking(IEnumerable<AdvisorMonthlyRanking> advisorsMonthlyRanking);
        List<AdvisorMonthlyRanking> ListAdvisorsHallOfFame(int topAmount);
    }
}
