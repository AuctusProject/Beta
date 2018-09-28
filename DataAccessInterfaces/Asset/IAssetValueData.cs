using Auctus.DomainObjects.Asset;
using System;
using System.Collections.Generic;

namespace Auctus.DataAccessInterfaces.Asset
{
    public interface IAssetValueData<T> : IBaseData<T>
    {
        List<AssetValue> Filter(IEnumerable<AssetValueFilter> filter);
    }
}