using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Asset;
using Auctus.DomainObjects.Exchange;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Auctus.Business.Asset
{
    public class AssetValueBusiness : BaseBusiness<AssetValue, IAssetValueData<AssetValue>>
    {
        public AssetValueBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        internal AssetValue LastAssetValue(int assetId)
        {
            return Data.GetLastValue(assetId);
        }

        internal List<AssetValue> FilterAssetValues(Dictionary<int, DateTime> assetsMap)
        {
            return Data.FilterAssetValues(assetsMap);
        }

        public void UpdateCoinmarketcapAssetsValues()
        {
            UpdateAssetsValues(CoinMarketcapBusiness.GetAllCoinsData(), IsCoinmarketcapAsset);
        }

        private bool IsCoinmarketcapAsset(DomainObjects.Asset.Asset asset, string key)
        {
            return asset.CoinMarketCapId == Convert.ToInt32(key);
        }

        public void UpdateCoingeckoAssetsValues()
        {
            UpdateAssetsValues(CoinGeckoBusiness.GetAllCoinsData(), IsCoingeckoAsset);
        }

        private bool IsCoingeckoAsset(DomainObjects.Asset.Asset asset, string key)
        {
            return asset.CoinGeckoId == key;
        }

        private void UpdateAssetsValues(IEnumerable<AssetResult> assetResults, Func<DomainObjects.Asset.Asset, string, bool> selectAssetFunc)
        {
            var currentDate = Data.GetDateTimeNow();
            currentDate = currentDate.AddMilliseconds(-currentDate.Millisecond);
            var assets = AssetBusiness.ListAssets();
            var assetValues = new List<AssetValue>();
            foreach (var assetValue in assetResults.Where(c => c.Price.HasValue))
            {
                var asset = assets.FirstOrDefault(c => selectAssetFunc(c, assetValue.Id));
                if (asset != null)
                    assetValues.Add(new AssetValue() { AssetId = asset.Id, Date = currentDate, Value = assetValue.Price.Value, MarketCap = assetValue.MarketCap });
            }
            Data.InsertManyAsync(assetValues);
        }
    }
}
