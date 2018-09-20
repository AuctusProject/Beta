using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Asset;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Auctus.DataAccessMock.Asset
{
    public class AssetData : BaseData<DomainObjects.Asset.Asset>, IAssetData<DomainObjects.Asset.Asset>
    {
        public static List<DomainObjects.Asset.Asset> AllAssets = new List<DomainObjects.Asset.Asset>()
        { 
            new DomainObjects.Asset.Asset() { Id = 1, Code = "BTC", Name = "Bitcoin", Type = AssetType.Crypto.Value, CoinMarketCapId = 1, ShortSellingEnabled = true },
            new DomainObjects.Asset.Asset() { Id = 2, Code = "ETH", Name = "Ethereum", Type = AssetType.Crypto.Value, CoinMarketCapId = 2, ShortSellingEnabled = true },
            new DomainObjects.Asset.Asset() { Id = 3, Code = "XRP", Name = "Ripple", Type = AssetType.Crypto.Value, CoinMarketCapId = 3, ShortSellingEnabled = false },
            new DomainObjects.Asset.Asset() { Id = 4, Code = "AUC", Name = "Auctus", Type = AssetType.Crypto.Value, CoinMarketCapId = 4, ShortSellingEnabled = false }
        };

        public override IEnumerable<DomainObjects.Asset.Asset> SelectAll()
        {
            return AllAssets;
        }

        public IEnumerable<DomainObjects.Asset.Asset> ListFollowingAssets(int userId)
        {
            var assetsIds = FollowAssetData.FollowAssetList.Where(c => c.UserId == userId).Select(c => c.AssetId);
            return SelectAll().Where(c => assetsIds.Contains(c.Id));
        }
    }
}
