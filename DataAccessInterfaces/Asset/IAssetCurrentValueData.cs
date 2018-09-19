using Auctus.DataAccessInterfaces;
using Auctus.DomainObjects.Asset;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessInterfaces.Asset
{
    public interface IAssetCurrentValueData<T> : IBaseData<T>
    {
        List<AssetCurrentValue> ListAllAssets(IEnumerable<int> ids);
    }
}
