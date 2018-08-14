using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Asset;
using DataAccessMock.Asset;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Auctus.DataAccessMock.Asset
{
    public class AssetData : BaseData<DomainObjects.Asset.Asset>, IAssetData<DomainObjects.Asset.Asset>
    {
        public override IEnumerable<DomainObjects.Asset.Asset> SelectAll()
        {
            var assets = new List<DomainObjects.Asset.Asset>();
            assets.Add(new DomainObjects.Asset.Asset() { Id = 1, Code = "BTC", Name = "Bitcoin", Type = AssetType.Crypto.Value, CoinMarketCapId = 1, ShortSellingEnabled = true });
            assets.Add(new DomainObjects.Asset.Asset() { Id = 2, Code = "ETH", Name = "Ethereum", Type = AssetType.Crypto.Value, CoinMarketCapId = 2, ShortSellingEnabled = true });
            assets.Add(new DomainObjects.Asset.Asset() { Id = 3, Code = "XRP", Name = "Ripple", Type = AssetType.Crypto.Value, CoinMarketCapId = 3, ShortSellingEnabled = false });
            assets.Add(new DomainObjects.Asset.Asset() { Id = 4, Code = "AUC", Name = "Auctus", Type = AssetType.Crypto.Value, CoinMarketCapId = 4, ShortSellingEnabled = false });
            return assets;
        }

        public IEnumerable<DomainObjects.Asset.Asset> ListFollowingAssets(int userId)
        {
            var assetsIds = FollowAssetData.FollowAssetList.Where(c => c.UserId == userId).Select(c => c.AssetId);
            return SelectAll().Where(c => assetsIds.Contains(c.Id));
        }
    }
}
