using Auctus.Business.Advisor;
using Auctus.DomainObjects.Advisor;
using Auctus.Model;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.Service
{
    public class AdvisorServices : BaseServices
    {
        public AdvisorServices(ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(loggerFactory, cache, email, ip) { }

        public void Advise(int userId, int assetId, AdviceType type)
        {
           AdvisorBusiness.Advise(userId, assetId, type);
        }
    }
}
