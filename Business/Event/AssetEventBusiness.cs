using Auctus.DataAccessInterfaces.Event;
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

namespace Auctus.Business.Event
{
    public class AssetEventBusiness : BaseBusiness<AssetEvent, IAssetEventData<AssetEvent>>
    {
        public AssetEventBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public List<AssetEvent> List(IEnumerable<int> assetsId, int? top = null, int? lastEventId = null)
        {
            var minimumReliablePercentage = 33;
            return Data.ListAssetEventsWithPagination(null, assetsId, top, lastEventId, minimumReliablePercentage, true);
        }

        public IEnumerable<FeedResponse> ListAssetEvents(int? top, int? lastEventId, int? assetId = null)
        {
            int[] assets = null;
            if (assetId.HasValue)
                assets = new int[] { assetId.Value };

            var events = Task.Factory.StartNew(() => List(assets, top, lastEventId));
            var user = GetLoggedUser();
            return UserBusiness.FillFeedList(null, null, events, user, top, null, null, lastEventId);
        }

        public EventResponse ConvertToEventResponse(AssetEvent assetEvent)
        {
            var categories = AssetEventCategoryBusiness.ListCategories().Where(c => assetEvent.LinkEventCategory.Any(a => a.AssetEventCategoryId == c.Id)).ToList();
            return new EventResponse()
            {
                EventId = assetEvent.Id,
                Title = assetEvent.Title,
                Description = assetEvent.Description,
                EventDate = assetEvent.EventDate,
                CreationDate = assetEvent.ExternalCreationDate,
                CanOccurBefore = assetEvent.CanOccurBefore,
                Source = assetEvent.Source,
                Categories = categories.Select(c => new EventResponse.EventCategoryResponse()
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            };
        }

        public async Task UpdateAssetEventsAsync()
        {
            var startDate = Data.GetDateTimeNow().AddDays(-1).Date;

            List<DomainObjects.Asset.Asset> assets = null;
            List<AssetEventCategory> categories = null;
            List<AssetEvent> assetEvents = null;
            List<Record> events = null;
            Parallel.Invoke(() => assets = AssetBusiness.ListAssets(), 
                () => categories = AssetEventCategoryBusiness.ListCategories(), 
                () => assetEvents = Data.ListAssetEventsWithPagination(startDate, null, null, null, null, null),
                () => events = CoinMarketCalBusiness.ListEvents(startDate));

            events = events.OrderBy(c => c.FormattedCreatedDate).ThenBy(c => c.FormattedEventDate).ToList();

            foreach(var e in events)
            {
                bool updateProofImage = false;
                bool isNewEvent = false;
                var linkCategoryToDelete = new List<LinkEventCategory>();
                var linkCategoryToInsert = new List<LinkEventCategory>();
                var linkAssetToDelete = new List<LinkEventAsset>();
                var linkAssetToInsert = new List<LinkEventAsset>();

                var coinsId = e.Coins.Select(c => c.Id).Distinct().ToHashSet();
                var eventAssets = assets.Where(c => coinsId.Contains(c.CoinMarketCalId)).ToList();
                var categoriesId = e.Categories.Select(c => c.Id).Distinct().ToHashSet();
                var eventCategories = categories.Where(c => categoriesId.Contains(c.Id)).ToList();

                if (eventAssets.Any())
                {
                    var workingEvent = assetEvents.FirstOrDefault(c => c.ExternalId == e.Id.ToString());
                    if (workingEvent == null)
                    {
                        isNewEvent = true;
                        updateProofImage = true;
                        workingEvent = new AssetEvent()
                        {
                            ExternalId = e.Id.ToString(),
                            CreationDate = Data.GetDateTimeNow()
                        };
                        linkCategoryToInsert = eventCategories.Select(c => new LinkEventCategory() { AssetEventCategoryId = c.Id }).ToList();
                        linkAssetToInsert = eventAssets.Select(c => new LinkEventAsset() { AssetId = c.Id }).ToList();
                    }
                    else
                    {
                        linkCategoryToDelete = workingEvent.LinkEventCategory.Where(c => !eventCategories.Any(a => a.Id == c.AssetEventCategoryId)).ToList();
                        linkAssetToDelete = workingEvent.LinkEventAsset.Where(c => !eventAssets.Any(a => a.Id == c.AssetId)).ToList();
                        linkCategoryToInsert = eventCategories.Where(c => !workingEvent.LinkEventCategory.Any(a => a.AssetEventCategoryId == c.Id)).Select(c => new LinkEventCategory()
                        {
                            AssetEventCategoryId = c.Id,
                            AssetEventId = workingEvent.Id
                        }).ToList();
                        linkAssetToInsert = eventAssets.Where(c => !workingEvent.LinkEventAsset.Any(a => a.AssetId == c.Id)).Select(c => new LinkEventAsset()
                        {
                            AssetId = c.Id,
                            AssetEventId = workingEvent.Id
                        }).ToList();

                        if (workingEvent.Title == e.Title && workingEvent.Description == e.Description && workingEvent.EventDate == e.FormattedEventDate &&
                            workingEvent.ExternalCreationDate == e.FormattedCreatedDate && workingEvent.CanOccurBefore == e.CanOccurBefore &&
                            workingEvent.ReliablePercentage == e.Percentage && workingEvent.Source == e.Source && workingEvent.Proof == e.Proof &&
                            linkCategoryToDelete.Count == 0 && linkAssetToDelete.Count == 0 && linkCategoryToInsert.Count == 0 && linkAssetToInsert.Count == 0)
                            continue;

                        updateProofImage = workingEvent.Proof != e.Proof;
                    }

                    UpdateAssetEventData(workingEvent, e);

                    try
                    {
                        using (var transaction = TransactionalDapperCommand)
                        {
                            if (isNewEvent)
                                transaction.Insert(workingEvent);
                            else
                                transaction.Update(workingEvent);

                            foreach (var linkCategory in linkCategoryToDelete)
                                transaction.Delete(linkCategory);
                            foreach (var linkCategory in linkCategoryToInsert)
                            {
                                linkCategory.AssetEventId = workingEvent.Id;
                                transaction.Insert(linkCategory);
                            }

                            foreach (var linkAsset in linkAssetToDelete)
                                transaction.Delete(linkAsset);
                            foreach (var linkAsset in linkAssetToInsert)
                            {
                                linkAsset.AssetEventId = workingEvent.Id;
                                transaction.Insert(linkAsset);
                            }

                            transaction.Commit();
                        }
                        if (updateProofImage)
                        {
                            var fileName = $"{workingEvent.Id}.png";
                            var contentType = workingEvent.Proof.ToLower().EndsWith("png") ? "image/png" : workingEvent.Proof.ToLower().EndsWith("pdf") ? "application/pdf" : "image/jpeg";
                            byte[] file;
                            using (var client = new WebClient())
                            {
                                file = client.DownloadData(workingEvent.Proof);
                            }
                            if (!await AzureStorageBusiness.UploadAssetEventFromByteAsync(fileName, file, contentType))
                                throw new BusinessException($"Error on upload asset event proof image. (Id {workingEvent.Id} - Url {workingEvent.Proof}");
                        }
                    }
                    catch (Exception ex)
                    {
                        var telemetry = new TelemetryClient();
                        telemetry.TrackEvent($"AssetEventBusiness.UpdateAssetEvents.Id.{e?.Id}");
                        telemetry.TrackException(ex);
                    }
                }
            }
        }

        private void UpdateAssetEventData(AssetEvent assetEvent, Record eventRecord)
        {
            assetEvent.Title = eventRecord.Title;
            assetEvent.Description = eventRecord.Description;
            assetEvent.CanOccurBefore = eventRecord.CanOccurBefore;
            assetEvent.EventDate = eventRecord.FormattedEventDate;
            assetEvent.ExternalCreationDate = eventRecord.FormattedCreatedDate;
            assetEvent.Proof = eventRecord.Proof;
            assetEvent.Source = eventRecord.Source;
            assetEvent.ReliablePercentage = eventRecord.Percentage;
            assetEvent.UpdateDate = Data.GetDateTimeNow();
        }
    }
}
