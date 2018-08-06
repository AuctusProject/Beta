using Auctus.DataAccess.Asset;
using Auctus.DataAccess.Exchanges;
using Auctus.Model;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.Business.Asset
{
    public class AssetBusiness : BaseBusiness<Auctus.DomainObjects.Asset.Asset, AssetData>
    {
        public AssetBusiness(ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(loggerFactory, cache, email, ip) { }

        public IEnumerable<AssetResponse> ListAssetData()
        {
            var user = GetValidUser();
            var advisors = AdvisorBusiness.GetAdvisors();
            var advices = Task.Factory.StartNew(() => AdviceBusiness.List(advisors.Select(c => c.Id).Distinct()));
            var assetFollowers = Task.Factory.StartNew(() => FollowAssetBusiness.List());
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
            var advisorFollowers = Task.Factory.StartNew(() => FollowAdvisorBusiness.List(advisors.Select(c => c.Id).Distinct()));
            var assetFollowers = Task.Factory.StartNew(() => FollowAssetBusiness.List());
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
