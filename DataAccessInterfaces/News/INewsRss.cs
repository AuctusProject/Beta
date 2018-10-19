using Auctus.DomainObjects.Event;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.DataAccessInterfaces.News
{
    public interface INewsRss
    {
        List<DomainObjects.News.News> ReadRssSource(string sourceUrl, int sourceId);
    }
}
