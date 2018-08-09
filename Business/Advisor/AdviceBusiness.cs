using Auctus.DataAccess.Advisor;
using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Advisor
{
    public class AdviceBusiness : BaseBusiness<Advice, IAdviceData<Advice>>
    {
        public AdviceBusiness(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(serviceProvider, loggerFactory, cache, email, ip) { }

        public void Advise(int assetId, AdviceType type)
        {
            var user = GetValidUser();
            if (!UserBusiness.IsValidAdvisor(user))
                throw new Exception("Logged user is not an Advisor.");

            var asset = AssetBusiness.GetById(assetId);
            if (asset == null)
                throw new Exception("Asset not found.");

            Insert(
                new Advice()
                {
                    AdvisorId = user.Id,
                    AssetId = assetId,
                    Type = type.Value,
                    CreationDate = Data.GetDateTimeNow()
                });
        }

        public List<Advice> List(IEnumerable<int> advisorsId)
        {
            return Data.List(advisorsId);
        }
    }
}
