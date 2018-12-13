using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Advisor
{
    public class AdvisorRankingData : BaseData<AdvisorRanking>, IAdvisorRankingData<AdvisorRanking>
    {
        public List<AdvisorRanking> ListAdvisorsRanking(IEnumerable<int> advisorsId)
        {
            throw new NotImplementedException();
        }

        public List<AdvisorRanking> ListAdvisorsRankingAndProfit(IEnumerable<int> advisorsId, IEnumerable<int> assetsId)
        {
            throw new NotImplementedException();
        }

        public void SetAdvisorRanking(IEnumerable<AdvisorRanking> advisorsRanking)
        {
            throw new NotImplementedException();
        }
    }
}
