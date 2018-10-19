using Auctus.DataAccessInterfaces.Event;
using Auctus.DataAccessInterfaces.News;
using Auctus.DomainObjects.Event;
using Auctus.Model;
using Auctus.Util;
using Auctus.Util.Exceptions;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Auctus.DomainObjects.Event.CoinMarketCalResult;

namespace Auctus.Business.News
{
    public class NewsSourceBusiness : BaseBusiness<DomainObjects.News.NewsSource, INewsSourceData<DomainObjects.News.NewsSource>>
    {
        public NewsSourceBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public IEnumerable<DomainObjects.News.NewsSource> List()
        {
            return Data.SelectAll();
        }
    }
}
