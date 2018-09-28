using Auctus.DomainObjects.Exchange;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessInterfaces.Exchange
{
    public interface ICoinGeckoApi
    {
        IEnumerable<AssetResult> GetAllCoinsData();
        AssetPricesResult GetAssetPrices(string assetId, int days);
        AssetDataResult GetCoinData(string assetId);
    }
}
