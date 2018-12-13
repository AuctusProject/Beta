using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Asset;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Auctus.DataAccess.Asset
{
    public class FollowAssetData : BaseSql<FollowAsset>, IFollowAssetData<FollowAsset>
    {
        public override string TableName => "FollowAsset";
        public FollowAssetData(IConfigurationRoot configuration) : base(configuration) { }

        private const string SQL_LIST = @"
		SELECT 
			f.*, fa.AssetId 
		FROM 
			[FollowAsset] fa WITH(NOLOCK)
			INNER JOIN [Follow] f WITH(NOLOCK) ON f.Id = fa.Id
			INNER JOIN (
				SELECT f2.UserId, fa2.AssetId, MAX(f2.CreationDate) CreationDate 
				FROM [FollowAsset] fa2 WITH(NOLOCK)
				INNER JOIN [Follow] f2 WITH(NOLOCK) ON f2.Id = fa2.Id 
				GROUP BY f2.UserId, fa2.AssetId) b 
		    ON b.UserId = f.UserId AND f.CreationDate = b.CreationDate AND b.AssetId = fa.AssetId
		WHERE 
			f.ActionType = @ActionType {0}";

        private const string SQL_LIST_ASSETS_FOLLOWED = @"
		SELECT 
			f.*, fa.AssetId 
		FROM 
			[FollowAsset] fa WITH(NOLOCK)
			INNER JOIN [Follow] f WITH(NOLOCK) ON f.Id = fa.Id
			INNER JOIN (
				SELECT f2.UserId, fa2.AssetId, MAX(f2.CreationDate) CreationDate 
				FROM [FollowAsset] fa2 WITH(NOLOCK)
				INNER JOIN [Follow] f2 WITH(NOLOCK) ON f2.Id = fa2.Id 
                WHERE f2.UserId = @UserId
				GROUP BY f2.UserId, fa2.AssetId) b 
		    ON b.UserId = f.UserId AND f.CreationDate = b.CreationDate AND b.AssetId = fa.AssetId
		WHERE 
			f.ActionType = @ActionType AND f.UserId = @UserId";

        private const string SQL_COUNT_ASSET_FOLLOWERS = @"
		SELECT 
			count(f.Id)
        FROM 
			[FollowAsset] fa WITH(NOLOCK)
			INNER JOIN [Follow] f WITH(NOLOCK) ON f.Id = fa.Id
			INNER JOIN (
				SELECT f2.UserId, fa2.AssetId, MAX(f2.CreationDate) CreationDate 
				FROM [FollowAsset] fa2 WITH(NOLOCK)
				INNER JOIN [Follow] f2 WITH(NOLOCK) ON f2.Id = fa2.Id 
                WHERE fa2.AssetId = @AssetId
				GROUP BY f2.UserId, fa2.AssetId) b 
		    ON b.UserId = f.UserId AND f.CreationDate = b.CreationDate AND b.AssetId = fa.AssetId
		WHERE 
			f.ActionType = @ActionType AND fa.AssetId = @AssetId";

        public List<FollowAsset> ListFollowers(IEnumerable<int> assetsIds)
        {
            var complement = "";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ActionType", FollowActionType.Follow.Value, DbType.Int32);
            if (assetsIds?.Count() > 0)
            {
                complement = $" AND ({string.Join(" OR ", assetsIds.Select((c, i) => $"fa.AssetId = @AssetId{i}"))})";
                for (int i = 0; i < assetsIds.Count(); ++i)
                    parameters.Add($"AssetId{i}", assetsIds.ElementAt(i), DbType.Int32);
            }
            return Query<FollowAsset>(string.Format(SQL_LIST, complement), parameters).ToList();
        }

        public List<FollowAsset> ListAssetsFollowed(int userId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ActionType", FollowActionType.Follow.Value, DbType.Int32);
            parameters.Add("UserId", userId, DbType.Int32);
            return Query<FollowAsset>(SQL_LIST_ASSETS_FOLLOWED, parameters).ToList();
        }

        public int CountAssetFollowers(int assetId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ActionType", FollowActionType.Follow.Value, DbType.Int32);
            parameters.Add("AssetId", assetId, DbType.Int32);
            return Query<int?>(SQL_COUNT_ASSET_FOLLOWERS, parameters).FirstOrDefault() ?? 0;
        }
    }
}
