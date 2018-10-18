using Auctus.DataAccessInterfaces.Exchange;
using Auctus.DomainObjects.Exchange;
using Auctus.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Auctus.Business.Exchange
{
    public class CoinGeckoBusiness
    {
        private readonly ICoinGeckoApi Api;

        internal CoinGeckoBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider)
        {
            Api = (ICoinGeckoApi)serviceProvider.GetService(typeof(ICoinGeckoApi));
        }

        public AssetDataResult GetFullCoinData(string assetId)
        {
            return Api.GetFullCoinData(assetId);
        }

        public AssetResult GetSimpleCoinData(string assetId)
        {
            return Api.GetSimpleCoinData(assetId);
        }

        public IEnumerable<AssetResult> GetAllCoinsData()
        {
            return Api.GetAllCoinsData();
        }

        public List<AssetResponse.ValuesResponse> GetAssetValues(string assetId, int days)
        {
            var assetPrices = GetAssetPrices(assetId, days);

            return assetPrices.Prices.Select(p => new AssetResponse.ValuesResponse() { Date = Util.Util.UnixMillisecondsTimeStampToDateTime(p[0]), Value = p[1] }).ToList();
        }

        private AssetPricesResult GetAssetPrices(string assetId, int days)
        {
            return Api.GetAssetPrices(assetId, days);
        }
    }
}
