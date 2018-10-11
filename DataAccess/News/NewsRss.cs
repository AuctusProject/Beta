using Auctus.DataAccess.Core;
using Auctus.DataAccess.News;
using Auctus.DataAccessInterfaces.News;
using Auctus.DomainObjects.News;
using Auctus.Util;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Auctus.DataAccess.News
{
    public class NewsRss : INewsRss
    {
        public NewsRss() { }

    
        private async Task<List<DomainObjects.News.News>> ReadRssSourceAsync(string sourceUrl, int sourceId)
        {
            var rssNewsItems = new List<DomainObjects.News.News>();
            
                using (var xmlReader = XmlReader.Create(sourceUrl, new XmlReaderSettings() { Async = true }))
                {
                    var feedReader = new RssFeedReader(xmlReader);
                    while (await feedReader.Read())
                    {
                        if (feedReader.ElementType == Microsoft.SyndicationFeed.SyndicationElementType.Item)
                        {
                            ISyndicationItem item = await feedReader.ReadItem();
                            rssNewsItems.Add(ConvertToNewsItem(item, sourceId));
                        }
                    }
                }
            
            return rssNewsItems;
        }

        private List<DomainObjects.News.News> ReadRssSourceSync(string sourceUrl, int sourceId)
        {
            var task = ReadRssSourceAsync(sourceUrl, sourceId);
            task.Wait();
            return task.Result;
        }

        public List<DomainObjects.News.News> ReadRssSource(string sourceUrl, int sourceId)
        {
            return Retry.Get().Execute<List<DomainObjects.News.News>>((Func<string, int, List<DomainObjects.News.News>>)ReadRssSourceSync, sourceUrl, sourceId);
        }

        private DomainObjects.News.News ConvertToNewsItem(ISyndicationItem item, int sourceId)
        {
            return new DomainObjects.News.News()
            {
                Title = item.Title.Truncate(255),
                Description = item.Description.Truncate(2000),
                Link = item.Links.FirstOrDefault()?.Uri.AbsoluteUri,
                ExternalCreationDate = item.Published.UtcDateTime,
                NewsCategory = item.Categories.Select(c => new NewsCategory() { Description = c.Name.Truncate(100) }).ToList(),
                ExternalId = item.Id,
                SourceId = sourceId
            };
        }
    }
}
