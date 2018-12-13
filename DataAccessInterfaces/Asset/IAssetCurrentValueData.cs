using Auctus.DataAccessInterfaces;
using Auctus.DomainObjects.Asset;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessInterfaces.Asset
{
    public interface IAssetCurrentValueData<T> : IBaseData<T>
    {
        List<AssetCurrentValue> ListAllAssets(bool enabled, IEnumerable<int> ids);
        List<AssetCurrentValue> ListAssetsFollowedByUser(int userId);
        void UpdateAssetValue(IEnumerable<AssetCurrentValue> assetCurrentValues);
        void UpdateFullAssetValue(IEnumerable<AssetCurrentValue> assetCurrentValues);
        void UpdateAssetValue7And30Days(IEnumerable<AssetCurrentValue> assetCurrentValues);
    }
}
