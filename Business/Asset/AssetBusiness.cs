using Auctus.DataAccess.Asset;
using Auctus.DataAccess.Exchanges;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auctus.Business.Asset
{
    public class AssetBusiness : BaseBusiness<Auctus.DomainObjects.Asset.Asset, AssetData>
    {
        public AssetBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }

        public List<Auctus.DomainObjects.Asset.Asset> ListAssets()
        {
            string cacheKey = "Assets";
            var assets = MemoryCache.Get<List<Auctus.DomainObjects.Asset.Asset>>(cacheKey);
            if (assets == null)
            {
                assets = Data.SelectAll().ToList();
                if (assets != null)
                    MemoryCache.Set<List<Auctus.DomainObjects.Asset.Asset>>(cacheKey, assets, 720);
            }
            return assets;
        }

        public void UpdateAllAssetsValues()
        {
            var assets = ListAssets();

            foreach (var asset in assets)
            {
                if (asset.Type == 1)
                    AssetValueBusiness.UpdateAssetValue(asset);
            }
        }

        public void CreateCoinMarketCapNotIncludedAssets()
        {
            var assets = AssetBusiness.ListAssets();
            var coinMarketCapAssets = new CoinMarketCapApi().GetAllCoinMarketCapAssets();

            foreach (var coinMarketCapAsset in coinMarketCapAssets)
            {
                if (!assets.Any(a => a.CoinMarketCapId == coinMarketCapAsset.Id))
                {
                    var assetCurrentValue = new DomainObjects.Asset.Asset()
                    {
                        Code = coinMarketCapAsset.Symbol,
                        CoinMarketCapId = coinMarketCapAsset.Id,
                        Name = coinMarketCapAsset.Name,
                        Type = DomainObjects.Asset.AssetType.Crypto.Value
                    };
                    Data.Insert(assetCurrentValue);
                }
            }
        }
    }
}
