using Auctus.DataAccess.Asset;
using Auctus.DataAccess.Core;
using Auctus.DataAccess.Exchanges;
using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Asset;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Auctus.Business.Asset
{
    public class AssetValueBusiness : BaseBusiness<AssetValue, IAssetValueData<AssetValue>>
    {
        public AssetValueBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, loggerFactory, cache, email, ip) { }

        internal AssetValue LastAssetValue(int assetId)
        {
            return Data.GetLastValue(assetId);
        }

        internal List<AssetValue> List(IEnumerable<int> assetsIds, DateTime startDate)
        {
            return Data.List(assetsIds, startDate);
        }

        public void UpdateAllAssetsValues()
        {
            var assets = AssetBusiness.ListAssets();
            var currentValuesDictionary = new CoinMarketCapApi().GetAllCoinsCurrentPrice();
            var currentDate = DateTime.UtcNow;
            currentDate = currentDate.AddMilliseconds(-currentDate.Millisecond);
            var assetValues = new List<DomainObjects.Asset.AssetValue>();

            foreach (var currentValue in currentValuesDictionary)
            {
                var asset = assets.FirstOrDefault(a => a.CoinMarketCapId == currentValue.Key);
                if (asset != null)
                    assetValues.Add(new DomainObjects.Asset.AssetValue() { AssetId = asset.Id, Date = currentDate, Value = currentValue.Value });
            }
            Data.InsertManyAsync(assetValues);
        }
    }
}
