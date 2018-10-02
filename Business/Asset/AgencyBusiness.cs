using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Asset;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auctus.Business.Asset
{
    public class AgencyBusiness : BaseBusiness<Agency, IAgencyData<Agency>>
    {
        public AgencyBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public List<Agency> All()
        {
            string cacheKey = "Agencies";
            var agencies = MemoryCache.Get<List<Agency>>(cacheKey);
            if (agencies == null)
            {
                agencies = Data.ListAll();
                if (agencies.Any())
                    MemoryCache.Set<List<Agency>>(cacheKey, agencies, 1440);
            }
            return agencies;
        }

        public Agency GetById(int id)
        {
            return All().FirstOrDefault(c => c.Id == id);
        }
    }
}
