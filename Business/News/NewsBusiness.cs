using Auctus.DataAccessInterfaces.News;
using Auctus.Util;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Auctus.Business.News
{
    public class NewsBusiness : BaseBusiness<DomainObjects.News.News, INewsData<DomainObjects.News.News>>
    {
        public NewsBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : 
            base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) {
        }

        public IEnumerable<DomainObjects.News.News> UpdateLastNews()
        {
            var notIncludedNews = GetNotIncludedNews();
            var savedNews = new List<DomainObjects.News.News>();
            foreach (var news in notIncludedNews)
            {
                try
                {
                    SaveNews(news);
                    savedNews.Add(news);
                }
                catch (Exception ex)
                {
                    var telemetry = new TelemetryClient();
                    telemetry.TrackEvent($"CreateNews.{news?.ExternalId}");
                    telemetry.TrackException(ex);
                }
            }
            return savedNews.OrderByDescending(n => n.Id);
        }

        public IEnumerable<DomainObjects.News.News> GetNotIncludedNews()
        {
            var newsSources = NewsSourceBusiness.ListAll();
            var lastNews = new List<DomainObjects.News.News>();
            foreach (var newsSource in newsSources)
            {
                try
                {
                    var news = NewsRssBusiness.List(newsSource.Url, newsSource.Id);
                    var notIncludeded = GetNotIncludedNews(news);
                    notIncludeded.ToList().ForEach(n => n.NewsSource = newsSource);
                    lastNews.AddRange(notIncludeded);
                }
                catch (Exception ex)
                {
                    var telemetry = new TelemetryClient();
                    telemetry.TrackEvent($"CreateNews.{newsSource?.Id}");
                    telemetry.TrackException(ex);
                }
            }
            return lastNews.OrderBy(n => n.ExternalCreationDate);
        }

        public IEnumerable<DomainObjects.News.News> ListNews(int? top, int? lastNewsId)
        {
            return Data.ListNewsWithPagination(top, lastNewsId);
        }

        private IEnumerable<DomainObjects.News.News> GetNotIncludedNews(IEnumerable<DomainObjects.News.News> news)
        {
            if (news?.Any() != true)
                return Enumerable.Empty<DomainObjects.News.News>();

            var includedNews = Data.ListNews(news.Select(n => n.ExternalId), news.FirstOrDefault().SourceId);
            return news.Where(n => !includedNews.Any(i => i.ExternalId == n.ExternalId));
        }

        private void SaveNews(IEnumerable<DomainObjects.News.News> newsToSave)
        {
            foreach (var news in newsToSave)
            {
                SaveNews(news);
            }
        }

        private void SaveNews(DomainObjects.News.News news)
        {
            news.CreationDate = Data.GetDateTimeNow();

            using (var transaction = TransactionalDapperCommand)
            {
                transaction.Insert(news);

                foreach (var newsCategory in news.NewsCategory)
                {
                    newsCategory.NewsId = news.Id;
                    transaction.Insert(newsCategory);
                }

                transaction.Commit();
            }
        }
    }
}
