using Auctus.DataAccessInterfaces;
using Auctus.DomainObjects.Asset;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessInterfaces.Asset
{
    public interface IFollowAssetData<T> : IBaseData<T>
    {
        List<FollowAsset> ListFollowers(IEnumerable<int> assetsIds);
        List<FollowAsset> ListAssetsFollowed(int userId);
        int CountAssetFollowers(int assetId);
    }
}
