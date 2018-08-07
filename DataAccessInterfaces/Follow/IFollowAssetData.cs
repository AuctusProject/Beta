using Auctus.DomainObjects.Follow;
using DataAccessInterfaces;
using System.Collections.Generic;

namespace Auctus.DataAccessInterfaces.Follow
{
    public interface IFollowAssetData<T> : IBaseData<T>
    {
        List<FollowAsset> List(IEnumerable<int> assetsIds);
    }
}