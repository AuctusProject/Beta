using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Asset;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Auctus.DomainObjects.Asset;

namespace Auctus.DataAccess.Asset
{
    public class AssetData : BaseSql<DomainObjects.Asset.Asset>, IAssetData<DomainObjects.Asset.Asset>
    {
        public override string TableName => "Asset";

        private const string SQL_LIST_FOLLOWING_ASSETS = @"SELECT a.* FROM 
            [Asset] a WITH(NOLOCK)
            INNER JOIN [FollowAsset] fa WITH(NOLOCK) ON fa.AssetId = a.Id
            INNER JOIN [Follow] f WITH(NOLOCK) ON f.Id = fa.Id
            INNER JOIN (
				SELECT f2.UserId, fa2.AssetId, MAX(f2.CreationDate) CreationDate 
				FROM [FollowAsset] fa2 WITH(NOLOCK)
				INNER JOIN [Follow] f2 WITH(NOLOCK) ON f2.Id = fa2.Id 
				GROUP BY f2.UserId, fa2.AssetId) b 
		    ON b.UserId = f.UserId AND f.CreationDate = b.CreationDate AND b.AssetId = fa.AssetId
             WHERE f.ActionType = @ActionType
	            AND f.UserId = @UserId";

        public AssetData(IConfigurationRoot configuration) : base(configuration) { }

        public IEnumerable<DomainObjects.Asset.Asset> ListFollowingAssets(int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("ActionType", DomainObjects.Account.FollowActionType.Follow.Value, DbType.Int32);
            parameters.Add("UserId", userId, DbType.Int32);

            return Query<DomainObjects.Asset.Asset>(SQL_LIST_FOLLOWING_ASSETS, parameters);
        }
    }
}
