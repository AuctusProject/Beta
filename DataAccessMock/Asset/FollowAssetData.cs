using Auctus.DataAccessMock;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Asset;
using DataAccessInterfaces.Asset;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessMock.Asset
{
    public class FollowAssetData : BaseData<FollowAsset>, IFollowAssetData<FollowAsset>
    {
        public List<FollowAsset> ListFollowers(IEnumerable<int> assetsIds)
        {
            var id = 0;
            var followers = new List<FollowAsset>
            {
                GetFollowers(ref id, 1, 1),
                GetFollowers(ref id, 2, 1),
                GetFollowers(ref id, 3, 1),
                GetFollowers(ref id, 4, 1),
                GetFollowers(ref id, 5, 1),
                GetFollowers(ref id, 1, 2)
            };
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
