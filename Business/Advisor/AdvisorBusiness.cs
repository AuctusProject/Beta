using Auctus.DataAccess.Advisor;
using Auctus.DataAccess.Core;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.Business.Advisor
{
    public class AdvisorBusiness : BaseBusiness<DomainObjects.Advisor.Advisor, AdvisorData>
    {
        public AdvisorBusiness(ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(loggerFactory, cache, email, ip) { }

        public void Advise(string userEmail, int assetId, AdviceType type)
        {
            var user = UserBusiness.GetValidUser(userEmail);

            if (!user.IsAdvisor)
                throw new Exception("Logged user is not an Advisor");

            AdviceBusiness.Insert(
                new Advice()
                {
                    UserId = user.Id,
                    AssetId = assetId,
                    Type = type.Value,
                    CreationDate = DateTime.UtcNow,
                });
        }
    }
}
