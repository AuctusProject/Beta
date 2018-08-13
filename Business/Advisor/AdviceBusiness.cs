using Auctus.DataAccess.Advisor;
using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;

namespace Auctus.Business.Advisor
{
    public class AdviceBusiness : BaseBusiness<Advice, IAdviceData<Advice>>
    {
        private const int MIN_TIME_BETWEEN_ADVICES_IN_SECONDS = 300;

        public AdviceBusiness(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(serviceProvider, loggerFactory, cache, email, ip) { }

        internal void ValidateAndCreate(int advisorId, int assetId, AdviceType type)
        {
            ValidateAdvice(assetId, advisorId, type);

            Insert(
                new Advice()
                {
                    AdvisorId = advisorId,
                    AssetId = assetId,
                    Type = type.Value,
                    CreationDate = Data.GetDateTimeNow()
                });
        }

        public List<Advice> List(IEnumerable<int> advisorsId)
        {
            return Data.List(advisorsId);
        }

        private Advice GetLastAdviceForAssetByAdvisor(int assetId, int advisorId)
        {
            return Data.GetLastAdviceForAssetByAdvisor(assetId, advisorId);
        }

        private void ValidateAdvice(int assetId, int advisorId, AdviceType type)
        {
            Advice lastAdvice = GetLastAdviceForAssetByAdvisor(assetId, advisorId);

            if(lastAdvice != null && Data.GetDateTimeNow().Subtract(lastAdvice.CreationDate).TotalSeconds < MIN_TIME_BETWEEN_ADVICES_IN_SECONDS)
                throw new InvalidOperationException("You need to wait before advising again for this asset.");

            if (type == AdviceType.ClosePosition)
            {
                if (lastAdvice == null || lastAdvice.AdviceType == AdviceType.ClosePosition)
                    throw new InvalidOperationException("You need a Buy or Sell recommendation before advising to Close Position.");
            }
        }
    }
}
