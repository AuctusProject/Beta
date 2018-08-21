using Auctus.DataAccessInterfaces.Asset;
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
            var user = GetValidUser();
            var advisors = AdvisorBusiness.GetAdvisors();
            var advices = Task.Factory.StartNew(() => AdviceBusiness.List(advisors.Select(c => c.Id).Distinct()));
            var assetFollowers = Task.Factory.StartNew(() => FollowAssetBusiness.ListFollowers());
            Task.WaitAll(advices, assetFollowers);

            List<AdvisorResponse> advisorsResult;
            List<AssetResponse> assetsResult;
            AdvisorBusiness.Calculation(Advisor.AdvisorBusiness.CalculationMode.AssetBase, out advisorsResult, out assetsResult, user, advices.Result, null, null, assetFollowers.Result);
            return assetsResult;
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
            AdvisorBusiness.Calculation(Advisor.AdvisorBusiness.CalculationMode.AssetDetailed, out advisorsResult, out assetsResult, user, advices.Result, advisors, advisorFollowers.Result, assetFollowers.Result);
            var result = assetsResult.Single(c => c.AssetId == assetId);
            result.Advisors = advisorsResult.Where(c => result.AssetAdvisor.Any(a => a.UserId == c.UserId)).ToList();
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

        public Auctus.DomainObjects.Asset.Asset GetById(int id)
        {
            return ListAssets(new int[] { id }).FirstOrDefault();
        }

        public void UpdateCoinmarketcapAssetsIcons()
        {
            UpdateIcons(CoinMarketcapBusiness.GetAllCoinsData(), IsCoinmarketcapAsset);
        }

        public void UpdateCoingeckoAssetsIcons()
        {
            UpdateIcons(CoinGeckoBusiness.GetAllCoinsData(), IsCoingeckoAsset);
        }

        public void CreateCoinmarketcapNotIncludedAssets()
        {
            CreateNotIncludedAssets(CoinMarketcapBusiness.GetAllCoinsData(), CoinGeckoBusiness.GetAllCoinsData(), false);
        }

        public void CreateCoingeckoNotIncludedAssets()
        {
            CreateNotIncludedAssets(CoinGeckoBusiness.GetAllCoinsData(), CoinMarketcapBusiness.GetAllCoinsData(), true);
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
            var assets = AssetBusiness.ListAssets();
            var assetValues = new List<AssetValue>();
            foreach (var assetValue in assetResults.Where(c => c.MarketCap.HasValue && c.MarketCap > 0))
            {
                var asset = assets.FirstOrDefault(c => selectAssetFunc(c, assetValue.Id));
                if (asset != null)
                {
                    asset.MarketCap = assetValue.MarketCap.Value;
                    Data.Update(asset);
                }
            }
        }

        private void CreateNotIncludedAssets(IEnumerable<AssetResult> assetResults, IEnumerable<AssetResult> assetExternalResults, bool isCoingecko)
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
                    Type = DomainObjects.Asset.AssetType.Crypto.Value
                };
                if (isCoingecko)
                    newAsset.CoinGeckoId = asset.Id;
                else
                    newAsset.CoinMarketCapId = Convert.ToInt32(asset.Id);

                Data.Insert(newAsset);
                UploadAssetIcon(newAsset.Id, asset.ImageUrl);
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

        private void UpdateIcons(IEnumerable<AssetResult> assetResults, Func<DomainObjects.Asset.Asset, string, bool> selectAssetFunc)
        {
            var assets = AssetBusiness.ListAll();
            foreach (var assetData in assetResults)
            {
                var asset = assets.FirstOrDefault(c => selectAssetFunc(c, assetData.Id));
                if (asset != null)
                    UploadAssetIcon(asset.Id, assetData.ImageUrl);
            }
        }

        private bool UploadAssetIcon(int assetId, string url)
        {
            url = !url.Contains('?') ? url : url.Split('?')[0];
            return AzureStorageBusiness.UploadAssetFromUrl($"{assetId}.png", url);
        }

        public IEnumerable<DomainObjects.Asset.Asset> ListFollowingAssets()
        {
            var user = GetValidUser();
            return Data.ListFollowingAssets(user.Id);
        }
    }
}
