using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Advisor;
using Auctus.DomainObjects.Asset;
using Auctus.DomainObjects.Exchange;
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

        public IEnumerable<AssetResponse> ListTrendingAssets(int top = 3)
        {
            var trendingAssetsIds = AdviceBusiness.ListTrendingAdvisedAssets(top).ToList();
            var trendingAssets = new List<AssetResponse>();
            var assets = ListAssetResult();

            trendingAssetsIds.ForEach(c => trendingAssets.Add(assets.FirstOrDefault(a => a.AssetId == c)));

            return trendingAssets;
        }

        private IEnumerable<AssetResponse> ListAssetResult()
        {
            var user = LoggedEmail != null ? UserBusiness.GetByEmail(LoggedEmail) : null;
            var advisors = AdvisorBusiness.GetAdvisors();
            var advices = Task.Factory.StartNew(() => AdviceBusiness.List(advisors.Select(c => c.Id).Distinct()));
            var assetFollowers = Task.Factory.StartNew(() => FollowAssetBusiness.ListFollowers());
            Task.WaitAll(advices, assetFollowers);

            List<AdvisorResponse> advisorsResult;
            List<AssetResponse> assetsResult;
            AdvisorBusiness.Calculation(Advisor.AdvisorBusiness.CalculationMode.AssetBase, out advisorsResult, out assetsResult, user, advices.Result, null, null, assetFollowers.Result);
            return assetsResult;
        }
        
        public AssetRecommendationInfoResponse GetAssetRecommendationInfo(int assetId)
        {
            var user = GetValidUser();
            var asset = GetById(assetId);
            //var lastAdvice = AdviceBusiness.GetLastAdviceForAssetByAdvisor(user.Id, assetId);

            var lastValue = AssetCurrentValueBusiness.GetCurrentValue(assetId);
            if (lastValue == null)
                throw new InvalidOperationException($"Asset {asset.Name} ({asset.Id}) does not have value defined.");

            return new AssetRecommendationInfoResponse()
            {
                AssetId = assetId,
                LastValue = lastValue.Value,
                CloseRecommendationEnabled = true //lastAdvice != null && lastAdvice.AdviceType != AdviceType.ClosePosition                
            };
        }

        public AssetResponse GetAssetData(int assetId)
        {
            var user = GetValidUser();
            var advisors = AdvisorBusiness.GetAdvisors();
            var advices = Task.Factory.StartNew(() => AdviceBusiness.List(advisors.Select(c => c.Id).Distinct()));
            var advisorFollowers = Task.Factory.StartNew(() => FollowAdvisorBusiness.ListFollowers(advisors.Select(c => c.Id).Distinct()));
            var assetFollowers = Task.Factory.StartNew(() => FollowAssetBusiness.ListFollowers());
            Task.WaitAll(advices, advisorFollowers, assetFollowers);

            List<AdvisorResponse> advisorsResult;
            List<AssetResponse> assetsResult;
            AdvisorBusiness.Calculation(Advisor.AdvisorBusiness.CalculationMode.AssetDetailed, out advisorsResult, out assetsResult, user, advices.Result, advisors, advisorFollowers.Result, assetFollowers.Result, assetId);
            var result = assetsResult.Single(c => c.AssetId == assetId);
            result.Advisors = advisorsResult.Where(c => result.AssetAdvisor.Any(a => a.UserId == c.UserId)).ToList();
            result.AssetAdvisor = result.AssetAdvisor.OrderByDescending(a => a.LastAdviceDate).ToList();
            return result;
        }

        public List<Auctus.DomainObjects.Asset.Asset> ListAssets(IEnumerable<int> ids = null)
        {
            string cacheKey = "Assets";
            var assets = MemoryCache.Get<List<Auctus.DomainObjects.Asset.Asset>>(cacheKey);
            if (assets == null)
            {
                assets = Data.SelectAll().ToList();
                if (assets != null)
                    MemoryCache.Set<List<Auctus.DomainObjects.Asset.Asset>>(cacheKey, assets, 720);
            }
            return ids == null ? assets : assets.Where(c => ids.Contains(c.Id)).ToList();
        }

        public List<Auctus.DomainObjects.Asset.Asset> ListAssetsOrderedByMarketCap(IEnumerable<int> ids = null)
        {
            return ListAssets().OrderByDescending(c => c.MarketCap ?? 0).ThenBy(c => c.Name).ToList();
        }

        public Auctus.DomainObjects.Asset.Asset GetById(int id)
        {
            return ListAssets(new int[] { id }).FirstOrDefault();
        }

        public async Task UpdateCoinmarketcapAssetsIconsAsync()
        {
            await UpdateIconsAsync(CoinMarketcapBusiness.GetAllCoinsData(), IsCoinmarketcapAsset);
        }

        public async Task UpdateCoingeckoAssetsIconsAsync()
        {
            await UpdateIconsAsync(CoinGeckoBusiness.GetAllCoinsData(), IsCoingeckoAsset);
        }

        public async Task CreateCoinmarketcapNotIncludedAssetsAsync()
        {
            await CreateNotIncludedAssetsAsync(CoinMarketcapBusiness.GetAllCoinsData(), CoinGeckoBusiness.GetAllCoinsData(), false);
        }

        public async Task CreateCoingeckoNotIncludedAssetsAsync()
        {
            await CreateNotIncludedAssetsAsync(CoinGeckoBusiness.GetAllCoinsData(), CoinMarketcapBusiness.GetAllCoinsData(), true);
        }

        private bool IsCoinmarketcapAsset(DomainObjects.Asset.Asset asset, string key)
        {
            return asset.CoinMarketCapId == Convert.ToInt32(key);
        }

        private bool IsCoingeckoAsset(DomainObjects.Asset.Asset asset, string key)
        {
            return asset.CoinGeckoId == key;
        }

        public void UpdateCoinmarketcapAssetsMarketcap()
        {
            UpdateAssetsMarketcap(CoinMarketcapBusiness.GetAllCoinsData(), IsCoinmarketcapAsset);
        }

        public void UpdateCoingeckoAssetsMarketcap()
        {
            UpdateAssetsMarketcap(CoinGeckoBusiness.GetAllCoinsData(), IsCoingeckoAsset);
        }

        private void UpdateAssetsMarketcap(IEnumerable<AssetResult> assetResults, Func<DomainObjects.Asset.Asset, string, bool> selectAssetFunc)
        {
            List<DomainObjects.Asset.Asset> assets = null;
            List<AssetCurrentValue> assetCurrentValues = null;
            Parallel.Invoke(() => assets = AssetBusiness.ListAssets(), () => assetCurrentValues = AssetCurrentValueBusiness.ListAllAssets());
            
            var pendingToInsertValue = assets.Where(c => !assetCurrentValues.Any(v => v.Id == c.Id));
            var resultList = assetResults.Where(c => (c.MarketCap.HasValue && c.MarketCap > 0) || (pendingToInsertValue.Any() && c.Price.HasValue));

            var assetsToUpdate = new List<DomainObjects.Asset.Asset>();
            var currentValuesToInsert = new List<AssetCurrentValue>();
            foreach (var assetValue in resultList)
            {
                var asset = assets.FirstOrDefault(c => selectAssetFunc(c, assetValue.Id));
                if (asset != null)
                {
                    if (assetValue.MarketCap.HasValue && assetValue.MarketCap > 0)
                    {
                        asset.MarketCap = assetValue.MarketCap;
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

        private async Task CreateNotIncludedAssetsAsync(IEnumerable<AssetResult> assetResults, IEnumerable<AssetResult> assetExternalResults, bool isCoingecko)
        {
            var assets = AssetBusiness.ListAll();
            foreach (var asset in assetResults)
            {
                if ((isCoingecko && assets.Any(c => c.CoinGeckoId == asset.Id))
                    || (!isCoingecko && assets.Any(c => c.CoinMarketCapId == Convert.ToInt32(asset.Id))))
                    continue;

                if (!asset.Price.HasValue)
                    continue;

                var assetsSameSymbol = assets.Where(c => c.Code.ToUpper() == asset.Symbol.ToUpper() && 
                    ((isCoingecko && string.IsNullOrEmpty(c.CoinGeckoId) && c.CoinMarketCapId.HasValue)
                    || (!isCoingecko && !string.IsNullOrEmpty(c.CoinGeckoId) && !c.CoinMarketCapId.HasValue)));

                if (ValidateSameAsset(asset, assetsSameSymbol, assetExternalResults, isCoingecko))
                    continue;

                var newAsset = new DomainObjects.Asset.Asset()
                {
                    Code = asset.Symbol.ToUpper(),
                    Name = asset.Name,
                    Type = DomainObjects.Asset.AssetType.Crypto.Value,
                    MarketCap = asset.MarketCap,
                    ShortSellingEnabled = true
                };
                if (isCoingecko)
                    newAsset.CoinGeckoId = asset.Id;
                else
                    newAsset.CoinMarketCapId = Convert.ToInt32(asset.Id);

                using (var transaction = TransactionalDapperCommand)
                {
                    transaction.Insert(newAsset);
                    transaction.Insert(new AssetCurrentValue() { CurrentValue = asset.Price.Value, Id = newAsset.Id, UpdateDate = Data.GetDateTimeNow() });
                    transaction.Commit();
                }
                await UploadAssetIconAsync(newAsset.Id, asset.ImageUrl);
            }
        }

        private bool ValidateSameAsset(AssetResult asset, IEnumerable<DomainObjects.Asset.Asset> assets, IEnumerable<AssetResult> assetExternalResults, bool isCoinGecko)
        {
            var isSame = false;
            foreach (var same in assets)
            {
                var externalSymbolAsset = assetExternalResults.First(c => (isCoinGecko && Convert.ToInt32(c.Id) == same.CoinMarketCapId)
                                                                || (!isCoinGecko && c.Id == same.CoinGeckoId));

                if (!externalSymbolAsset.Price.HasValue)
                    return true;

                if (IsSameAsset(asset, externalSymbolAsset))
                {
                    if (isCoinGecko)
                        same.CoinGeckoId = asset.Id;
                    else
                        same.CoinMarketCapId = Convert.ToInt32(asset.Id);

                    Data.Update(same);
                    isSame = true;
                    break;
                }
            }
            return isSame;
        }

        private bool IsSameAsset(AssetResult asset, AssetResult externalAsset)
        {
            return asset.Name.ToLowerInvariant() == externalAsset.Name.ToLowerInvariant() ||
                (Util.Util.IsEqualWithTolerance(asset.Price.Value, externalAsset.Price.Value, 0.02) && 
                (!asset.MarketCap.HasValue || !externalAsset.MarketCap.HasValue
                        || Util.Util.IsEqualWithTolerance(asset.MarketCap.Value, externalAsset.MarketCap.Value, 0.1)));
        }

        private async Task UpdateIconsAsync(IEnumerable<AssetResult> assetResults, Func<DomainObjects.Asset.Asset, string, bool> selectAssetFunc)
        {
            var assets = AssetBusiness.ListAll();
            foreach (var assetData in assetResults)
            {
                var asset = assets.FirstOrDefault(c => selectAssetFunc(c, assetData.Id));
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

        public IEnumerable<DomainObjects.Asset.Asset> ListFollowingAssets()
        {
            var user = GetValidUser();
            return Data.ListFollowingAssets(user.Id);
        }

        public IEnumerable<DomainObjects.Asset.Asset> ListByNameOrCode(string searchTerm)
        {
            return ListAssets().Where(asset => asset.Name.ToUpper().StartsWith(searchTerm.ToUpper()) || asset.Code.ToUpper().StartsWith(searchTerm.ToUpper()));
        }
    }
}
