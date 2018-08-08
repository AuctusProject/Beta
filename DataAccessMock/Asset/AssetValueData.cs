using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Asset;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Asset
{
    public class AssetValueData : BaseData<AssetValue>, IAssetValueData<AssetValue>
    {
        public AssetValue GetLastValue(int assetId)
        {
            throw new NotImplementedException();
        }

        public List<AssetValue> List(IEnumerable<int> assetsIds, DateTime? startDate = null)
        {
            throw new NotImplementedException();
        }
    }
}
