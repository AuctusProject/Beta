using Auctus.DataAccessInterfaces.Follow;
using Auctus.DomainObjects.Follow;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Follow
{
    public class FollowAssetData : BaseData<FollowAsset>, IFollowAssetData<FollowAsset>
    {
        public List<FollowAsset> List(IEnumerable<int> assetsIds)
        {
            throw new NotImplementedException();
        }
    }
}
