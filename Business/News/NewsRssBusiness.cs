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
    public class NewsRssBusiness
    {
        private readonly INewsRss Rss;

        public NewsRssBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider)
        {
            Rss = (INewsRss)serviceProvider.GetService(typeof(INewsRss));
        }

        public List<DomainObjects.News.News> List(string url, int sourceId)
        {
            return Rss.ReadRssSource(url, sourceId);
        }
    }
}
