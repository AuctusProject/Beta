using Auctus.DataAccessInterfaces.Event;
using Auctus.DomainObjects.Event;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Event
{
    public class AssetEventCategoryBusiness : BaseBusiness<AssetEventCategory, IAssetEventCategoryData<AssetEventCategory>>
    {
        public AssetEventCategoryBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

    }
}
