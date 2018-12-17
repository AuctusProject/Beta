using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
using Auctus.DomainObjects.Asset;
using Auctus.DomainObjects.Exchange;
using Auctus.DomainObjects.Trade;
using Auctus.Model;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.Business.Asset
{
    public class AssetBusiness : BaseBusiness<DomainObjects.Asset.Asset, IAssetData<DomainObjects.Asset.Asset>>
    {
        public AssetBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public IEnumerable<AssetResponse> ListAssetData()
        {
            return ListAssetResult().OrderByDescending(c => c.MarketCap);
        }

        public IEnumerable<TerminalAssetResponse> ListAssetsForTerminal()
        {
            string cacheKey = "AssetsForTerminal";
            var terminalAssets = MemoryCache.Get<List<TerminalAssetResponse>>(cacheKey);
            if (terminalAssets == null)
            {
                var consideredAssets = TerminalAssets;
                var ids = consideredAssets.Select(c => c.AssetId).Distinct().ToList();
                var assets = ListAssets(true, ids);
                if (assets != null && assets.Any())
                {
                    assets = assets.OrderBy(c => ids.IndexOf(c.Id)).ToList();
                    terminalAssets = assets.Select(c => new TerminalAssetResponse()
                    {
                        AssetId = c.Id,
                        Code = c.Code,
                        Name = c.Name,
                        ChartPair = consideredAssets.First(a => a.AssetId == c.Id).ChartPair,
                        ChartExchange = consideredAssets.First(a => a.AssetId == c.Id).ChartExchange
                    }).ToList();
                    MemoryCache.Set<List<TerminalAssetResponse>>(cacheKey, terminalAssets, 1440);
                }
            }
            return terminalAssets;
        }
        
        public AssetStatusResponse ListAssetStatus(int assetId)
        {
            AssetStatusResponse result = null;
            var asset = GetById(assetId);
            var coinGeckoData = CoinGeckoBusiness.GetSimpleCoinData(asset.CoinGeckoId);
            if (coinGeckoData != null)
            {
                result = new AssetStatusResponse()
                {
                    AssetId = asset.Id,
                    Name = asset.Name,
                    Code = asset.Code,
                    AllTimeHigh = coinGeckoData.AllTimeHigh,
                    AllTimeHighDate = coinGeckoData.AllTimeHighDate,
                    AllTimeHighPercentage = coinGeckoData.AllTimeHighPercentage,
                    CirculatingSupply = coinGeckoData.CirculatingSupply,
                    High24h = coinGeckoData.High24h,
                    Low24h = coinGeckoData.Low24h,
                    MarketCap = coinGeckoData.MarketCap,
                    MarketCapPercentage24h = coinGeckoData.MarketCapPercentage24h,
                    MarketCapVariation24h = coinGeckoData.MarketCapVariation24h,
                    MarketCapRank = coinGeckoData.MarketCapRank,
                    Price = coinGeckoData.Price,
                    TotalSupply = coinGeckoData.TotalSupply,
                    TotalVolume = coinGeckoData.TotalVolume,
                    Variation24h = coinGeckoData.Variation24h,
                    VariationPercentage24h = coinGeckoData.VariationPercentage24h
                };
            }
            return result;
        }

        public AssetResponse ListAssetBaseData(int assetId)
        {
            AssetResponse assetResponse = null;
            List<Report> reports = null;
            Parallel.Invoke(() => assetResponse = ListAssetResult(assetId).FirstOrDefault(),
                            () => reports = ReportBusiness.List(new int[] { assetId }));

            if (assetResponse != null)
                assetResponse.ReportRecommendationDistribution = ReportBusiness.GetReportRecommendationDistribution(reports);
            return assetResponse;
        }

        private IEnumerable<AssetResponse> ListAssetResult(int? forcedAssetId = null)
        {
            var selectAssets = forcedAssetId.HasValue ? new int[] { forcedAssetId.Value } : null;
            List<AssetCurrentValue> assets = null;
            List<AdvisorRanking> advisors = null;
            Parallel.Invoke(() => assets = AssetCurrentValueBusiness.ListAllAssets(true, selectAssets),
                            () => advisors = AdvisorRankingBusiness.ListAdvisorsFullData());
            
            var user = GetLoggedUser();
            return assets.Select(c => GetAssetResponse(user, c, null, advisors));
        }

        private AssetResponse GetAssetResponse(User loggedUser, AssetCurrentValue assetCurrentValue, int? totalFollowers, List<AdvisorRanking> advisors)
        {
            var totalAdvisors = advisors != null ? advisors.Where(c => c.AdvisorProfit.Any(a => a.AssetId == assetCurrentValue.Id && a.OrderStatusType != OrderStatusType.Open)) : null;
            var ratingAdvisors = advisors != null ? advisors.SelectMany(c => c.AdvisorProfit.Where(a => a.AssetId == assetCurrentValue.Id && a.OrderStatusType == OrderStatusType.Executed).Select(a => a)) : null;
            var distribution = ratingAdvisors != null && ratingAdvisors.Any() ? ratingAdvisors.GroupBy(c => c.Type).Select(g => new RecommendationDistributionResponse() { Type = g.Key, Total = g.Sum(c => c.OrderCount) }).ToList() : new List<RecommendationDistributionResponse>();
            var mode = GetAssetModeType(distribution);
            return new AssetResponse()
            {
                AssetId = assetCurrentValue.Id,
                Code = assetCurrentValue.Code,
                Name = assetCurrentValue.Name,
                Mode = mode,
                Following = loggedUser?.FollowedAssets?.Any(c => c == assetCurrentValue.Id),
                ShortSellingEnabled = assetCurrentValue.ShortSellingEnabled,
                NumberOfFollowers = totalFollowers,
                TotalAdvisors = totalAdvisors != null && totalAdvisors.Any() ? totalAdvisors.Select(c => c.Id).Distinct().Count() : 0,
                TotalRatings = ratingAdvisors != null && ratingAdvisors.Any() ? ratingAdvisors.Sum(c => c.OrderCount) : 0,
                LastValue = assetCurrentValue.CurrentValue,
                AskValue = assetCurrentValue.AskValue,
                BidValue = assetCurrentValue.BidValue,
                MarketCap = assetCurrentValue.MarketCap,
                CirculatingSupply = assetCurrentValue.CirculatingSupply,
                Variation24h = assetCurrentValue.Variation24Hours,
                Variation7d = assetCurrentValue.Variation7Days,
                Variation30d = assetCurrentValue.Variation30Days,
                Pair = PairBusiness.GetBaseQuotePair(assetCurrentValue.Id),
                RecommendationDistribution = distribution
            };
        }

        private int GetAssetModeType(List<RecommendationDistributionResponse> recommendationDistribution)
        {
            if (recommendationDistribution.Any())
            {
                var total = recommendationDistribution.Sum(c => c.Total);
                var percentages = recommendationDistribution.Select(c => new { Type = c.Type, Percentage = 100.0 * c.Total / total });
                var majorityType = percentages.SingleOrDefault(c => c.Percentage > 50);
                if (majorityType != null)
                {
                    if (total < 10 || majorityType.Percentage < 75)
                        return majorityType.Type == OrderType.Buy.Value ? AssetModeType.ModerateBuy.Value : AssetModeType.ModerateSell.Value;
                    else
                        return majorityType.Type == OrderType.Buy.Value ? AssetModeType.StrongBuy.Value : AssetModeType.StrongSell.Value;
                }
            }
            return AssetModeType.Neutral.Value;
        }

        public AssetResponse GetAssetData(int assetId)
        {
            AssetResponse asset = null;
            int totalFollowers = 0;
            Parallel.Invoke(() => asset = ListAssetResult(assetId).FirstOrDefault(),
                            () => totalFollowers = FollowAssetBusiness.GetTotalFollowers(assetId));

            if (asset != null)
                asset.NumberOfFollowers = totalFollowers;
            return asset;
        }

        public List<DomainObjects.Asset.Asset> ListAssets(bool enabled, IEnumerable<int> ids = null)
        {
            string cacheKey = "Assets";
            var assets = MemoryCache.Get<List<Auctus.DomainObjects.Asset.Asset>>(cacheKey);
            if (assets == null)
            {
                assets = Data.SelectAll().ToList();
                if (assets != null)
                    MemoryCache.Set<List<Auctus.DomainObjects.Asset.Asset>>(cacheKey, assets, 720);
            }
            IEnumerable<DomainObjects.Asset.Asset> result;
            if (enabled)
                result = assets.Where(c => c.Enabled);
            else
                result = assets;

            return ids == null ? result.ToList() : result.Where(c => ids.Contains(c.Id)).ToList();
        }

        public List<SimpleAssetResponse> ListAssetsOrderedByMarketCap()
        {
            return ListAssets(true).Select(c => new SimpleAssetResponse()
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                Code = c.Code,
                MarketCap = c.MarketCap,
                CirculatingSupply = c.CirculatingSupply,
                ShortSellingEnabled = c.ShortSellingEnabled,
                Pair = PairBusiness.GetBaseQuotePair(c.Id)
            }).OrderByDescending(c => c.MarketCap ?? 0).ThenBy(c => c.Name).ToList();
        }

        public DomainObjects.Asset.Asset GetById(int id)
        {
            return ListAssets(false, new int[] { id }).FirstOrDefault();
        }

        public async Task UpdateCoingeckoAssetsIconsAsync()
        {
            await UpdateIconsAsync(CoinGeckoBusiness.GetAllCoinsData());
        }

        public async Task CreateCoingeckoNotIncludedAssetsAsync()
        {
            await CreateNotIncludedAssetsAsync(CoinGeckoBusiness.GetAllCoinsData());
        }

        public void UpdateCoingeckoAssetsMarketcap()
        {
            UpdateAssetsMarketcap(CoinGeckoBusiness.GetAllCoinsData());
        }

        private void UpdateAssetsMarketcap(IEnumerable<AssetResult> assetResults)
        {
            List<DomainObjects.Asset.Asset> assets = null;
            List<AssetCurrentValue> assetCurrentValues = null;
            Parallel.Invoke(() => assets = AssetBusiness.ListAssets(true), () => assetCurrentValues = AssetCurrentValueBusiness.ListAllAssets(true));
            
            var pendingToInsertValue = assets.Where(c => !assetCurrentValues.Any(v => v.Id == c.Id));
            var resultList = assetResults.Where(c => (c.MarketCap.HasValue && c.MarketCap > 0) || (pendingToInsertValue.Any() && c.Price.HasValue));

            var assetsToUpdate = new List<DomainObjects.Asset.Asset>();
            var currentValuesToInsert = new List<AssetCurrentValue>();
            foreach (var assetValue in resultList)
            {
                var asset = assets.FirstOrDefault(c => c.CoinGeckoId == assetValue.Id);
                if (asset != null)
                {
                    if (assetValue.MarketCap.HasValue || assetValue.CirculatingSupply.HasValue)
                    {
                        var update = false;
                        if (assetValue.MarketCap.HasValue && assetValue.MarketCap != asset.MarketCap && assetValue.MarketCap > 0)
                        {
                            asset.MarketCap = assetValue.MarketCap;
                            update = true;
                        }
                        if (assetValue.CirculatingSupply.HasValue && assetValue.CirculatingSupply != asset.CirculatingSupply && assetValue.CirculatingSupply > 0)
                        { 
                            asset.CirculatingSupply = assetValue.CirculatingSupply;
                            update = true;
                        }
                        if (update)
                            assetsToUpdate.Add(asset);
                    }
                    if (assetValue.Price.HasValue && pendingToInsertValue.Any(c => c.Id == asset.Id))
                        currentValuesToInsert.Add(new AssetCurrentValue() { CurrentValue = assetValue.Price.Value, Id = asset.Id, UpdateDate = Data.GetDateTimeNow() });
                }
            }
            using (var transaction = TransactionalDapperCommand)
            {
                foreach (var asset in assetsToUpdate)
                    transaction.Update(asset);
                foreach (var value in currentValuesToInsert)
                    transaction.Insert(value);

                transaction.Commit();
            }
        }

        private async Task CreateNotIncludedAssetsAsync(IEnumerable<AssetResult> assetResults)
        {
            var assets = AssetBusiness.ListAll();
            foreach (var asset in assetResults)
            {
                if (!asset.Price.HasValue)
                    continue;

                if (assets.Any(c => c.CoinGeckoId == asset.Id))
                    continue;

                var newAsset = new DomainObjects.Asset.Asset()
                {
                    Code = asset.Symbol.ToUpper(),
                    Name = asset.Name,
                    Type = AssetType.Crypto.Value,
                    MarketCap = asset.MarketCap,
                    CirculatingSupply = asset.CirculatingSupply,
                    ShortSellingEnabled = true,
                    CoinGeckoId = asset.Id
                };

                using (var transaction = TransactionalDapperCommand)
                {
                    transaction.Insert(newAsset);
                    transaction.Insert(new AssetCurrentValue() { CurrentValue = asset.Price.Value, Id = newAsset.Id, UpdateDate = Data.GetDateTimeNow() });
                    transaction.Commit();
                }
                await UploadAssetIconAsync(newAsset.Id, asset.ImageUrl);
            }
        }

        private async Task UpdateIconsAsync(IEnumerable<AssetResult> assetResults)
        {
            var assets = AssetBusiness.ListAll();
            foreach (var assetData in assetResults)
            {
                var asset = assets.FirstOrDefault(c => c.CoinGeckoId == assetData.Id);
                if (asset != null)
                    await UploadAssetIconAsync(asset.Id, assetData.ImageUrl);
            }
        }

        private async Task<bool> UploadAssetIconAsync(int assetId, string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            if (!url.ToLower().StartsWith("http"))
                return false;

            if (url.ToLower().Contains("/large/"))
                url = url.Replace("/large/", "/small/");

            var result = await AzureStorageBusiness.UploadAssetFromUrlAsync($"{assetId}.png", url);

            if (!result && url.Contains("/small/"))
            {
                url = url.Replace("/small/", "/large/");
                result = await AzureStorageBusiness.UploadAssetFromUrlAsync($"{assetId}.png", url);
            }
            return result;
        }

        public IEnumerable<AssetResponse> ListAssetsFollowedByUser(int userId)
        {
            var assetValues = AssetCurrentValueBusiness.ListAssetsFollowedByUser(userId); 
            var advisors = AdvisorRankingBusiness.ListAdvisorsFullData();
            var loggedUser = GetValidUser();
            var assetResponse = assetValues.Select(asset => GetAssetResponse(loggedUser, asset, null, advisors));

            return assetResponse.OrderByDescending(asset => asset.MarketCap);
        }

        public IEnumerable<DomainObjects.Asset.Asset> ListByNameOrCode(string searchTerm)
        {
            return ListAssets(true).Where(asset => asset.Name.ToUpper().StartsWith(searchTerm.ToUpper()) || asset.Code.ToUpper().StartsWith(searchTerm.ToUpper()));
        }

        public IEnumerable<AssetResponse> ListTrendingAssets(int? listSize)
        {
            var numberOfRecordsInResult = listSize ?? 10;
            var numberOfDays = 7;
            var statusList = new int[3] { OrderStatusType.Executed.Value, OrderStatusType.Close.Value, OrderStatusType.Finished.Value };

            var ids = OrderBusiness.ListTrendingAssetIdsBasedOnOrders(statusList, numberOfRecordsInResult, numberOfDays).ToList();

            var assets = AssetCurrentValueBusiness.ListAllAssets(true, ids);

            var loggedUser = GetLoggedUser();
            var assetResponse = assets.Select(asset => GetAssetResponse(loggedUser, asset, null, null)).OrderBy(asset => ids.IndexOf(asset.AssetId));

            return assetResponse;
        }
    }
}
