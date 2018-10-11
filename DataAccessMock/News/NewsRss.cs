using Auctus.DataAccessInterfaces.Event;
using Auctus.DataAccessInterfaces.News;
using Auctus.DomainObjects.Event;
using Auctus.DomainObjects.News;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Event
{
    public class NewsRss : INewsRss
    {
        public List<News> ReadRssSource(string sourceUrl, int sourceId)
        {
            return new List<News>();
        }
    }
}
