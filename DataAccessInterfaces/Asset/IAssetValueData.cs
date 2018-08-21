using Auctus.DomainObjects.Asset;
using System;
using System.Collections.Generic;

namespace Auctus.DataAccessInterfaces.Asset
{
    public interface IAssetValueData<T> : IBaseData<T>
    {
        AssetValue GetLastValue(int assetId);
        List<AssetValue> List(IEnumerable<int> assetsIds, DateTime? startDate = null);

        List<AssetValue> FilterAssetValues(Dictionary<int, DateTime> assetsMap);
    }
}