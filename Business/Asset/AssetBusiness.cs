using Auctus.DataAccess.Asset;
using Auctus.DataAccess.Exchanges;
using Auctus.DataAccessInterfaces.Asset;
using Auctus.Model;
using Auctus.Util;
using Auctus.Util.Azure;
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
        public readonly static string COINMARKETCAP_ICONS_BASE_URL = @"https://s2.coinmarketcap.com/static/img/coins/32x32/{0}";
        public readonly static string ICON_CONTAINER_NAME = "assetsicons";
        public AssetBusiness(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(serviceProvider, loggerFactory, cache, email, ip) { }

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
            var result = assetsResult.Where(c => c.AssetId == assetId).Single();
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
            return ids != null ? assets : assets.Where(c => ids.Contains(c.Id)).ToList();
        }

        public Auctus.DomainObjects.Asset.Asset GetById(int id)
        {
            return ListAssets(new int[] { id }).FirstOrDefault();
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
