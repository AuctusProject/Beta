using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Asset;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Asset
{
    public class AssetCurrentValueData : BaseData<DomainObjects.Asset.AssetCurrentValue>, IAssetCurrentValueData<DomainObjects.Asset.AssetCurrentValue>
    {
        public List<AssetCurrentValue> ListAllAssets(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public void UpdateAssetValue(List<AssetCurrentValue> assetCurrentValues)
        {
            throw new NotImplementedException();
        }
    }
}
