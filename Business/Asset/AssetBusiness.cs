using Auctus.DataAccess.Asset;
using Auctus.DataAccess.Exchanges;
using Auctus.Util;
using Auctus.Util.Azure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auctus.Business.Asset
{
    public class AssetBusiness : BaseBusiness<Auctus.DomainObjects.Asset.Asset, AssetData>
    {
        public readonly static string COINMARKETCAP_ICONS_BASE_URL = @"https://s2.coinmarketcap.com/static/img/coins/32x32/{0}";
        public readonly static string ICON_CONTAINER_NAME = "assetsicons";
        public AssetBusiness(ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(loggerFactory, cache, email, ip) { }

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

        public Auctus.DomainObjects.Asset.Asset GetById(int id)
        {
            return ListAssets().Where(x => x.Id == id).FirstOrDefault();
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

        public void UpdateAllAssetsIcons()
        {
            var coinMarketCapAssets = new CoinMarketCapApi().GetAllCoinMarketCapAssets();
            foreach (var coinMarketCapAsset in coinMarketCapAssets)
            {
                UploadAssetIcon(string.Format("{0}.png", coinMarketCapAsset.Id));
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
                    UploadAssetIcon(string.Format("{0}.png", coinMarketCapAsset.Id));
                }
            }
        }

        private void UploadAssetIcon(string fileName)
        {
            StorageManager.UploadFileFromUrl(ICON_CONTAINER_NAME,fileName,string.Format(COINMARKETCAP_ICONS_BASE_URL, fileName));
        }
    }
}
