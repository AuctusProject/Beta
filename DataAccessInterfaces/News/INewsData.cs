using Auctus.DomainObjects.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessInterfaces.News
{
    public interface INewsData<News> : IBaseData<News>
    {
        IEnumerable<News> ListNews(IEnumerable<string> externalIds, int sourceId);

        IEnumerable<News> ListNewsWithPagination(int? top, int? lastNewsId);
    }
}
