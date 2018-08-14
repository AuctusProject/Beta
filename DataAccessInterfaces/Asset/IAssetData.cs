using System.Collections.Generic;

namespace Auctus.DataAccessInterfaces.Asset
{
    public interface IAssetData<T> : IBaseData<T>
    {
        IEnumerable<DomainObjects.Asset.Asset> ListFollowingAssets(int userId);
    }
}