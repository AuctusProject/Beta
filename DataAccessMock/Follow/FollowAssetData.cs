using Auctus.DataAccessInterfaces.Follow;
using Auctus.DomainObjects.Follow;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Follow
{
    public class FollowAssetData : BaseData<FollowAsset>, IFollowAssetData<FollowAsset>
    {
        public List<FollowAsset> ListFollowers(IEnumerable<int> assetsIds)
        {
            var id = 0;
            var followers = new List<FollowAsset>();
            followers.Add(GetFollowers(ref id, 1, 1));
            followers.Add(GetFollowers(ref id, 2, 1));
            followers.Add(GetFollowers(ref id, 3, 1));
            followers.Add(GetFollowers(ref id, 4, 1));
            followers.Add(GetFollowers(ref id, 5, 1));
            followers.Add(GetFollowers(ref id, 1, 2));
            return followers;
        }

        private FollowAsset GetFollowers(ref int id, int userId, int assetId)
        {
            ++id;
            return new FollowAsset()
            {
                Id = id,
                UserId = userId,
                AssetId = assetId,
                ActionType = FollowActionType.Follow.Value,
                CreationDate = DateTime.UtcNow
            };
        }
    }
}
