using Auctus.DataAccessInterfaces.Event;
using Auctus.DataAccessInterfaces.News;
using Auctus.DomainObjects.Event;
using Auctus.DomainObjects.News;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Event
{
    public class NewsData : BaseData<News>, INewsData<News>
    {
        public IEnumerable<News> ListNews(IEnumerable<string> externalIds, int sourceId)
        {
            throw new NotImplementedException();
        }
    }
}
