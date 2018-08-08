using Auctus.DataAccessInterfaces.Asset;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Asset
{
    public class AssetData : BaseData<DomainObjects.Asset.Asset>, IAssetData<DomainObjects.Asset.Asset>
    {
    }
}
