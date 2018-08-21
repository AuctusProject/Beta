using Auctus.DataAccessInterfaces.Asset;
using Auctus.DataAccessMock;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Asset;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Asset
{
    public class FollowAssetData : BaseData<FollowAsset>, IFollowAssetData<FollowAsset>
    {
        public static List<FollowAsset> FollowAssetList = CreateFollowAssetList();

        private static List<FollowAsset> CreateFollowAssetList()
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

        public List<FollowAsset> ListFollowers(IEnumerable<int> assetsIds)
        {
            return FollowAssetList;
        }

        private static FollowAsset GetFollowers(ref int id, int userId, int assetId)
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
