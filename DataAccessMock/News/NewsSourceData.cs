using Auctus.DataAccessInterfaces.Event;
using Auctus.DataAccessInterfaces.News;
using Auctus.DomainObjects.Event;
using Auctus.DomainObjects.News;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Event
{
    public class NewsSourceData : BaseData<NewsSource>, INewsSourceData<NewsSource>
    {
    }
}
