using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Asset;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Auctus.DomainObjects.Asset;

namespace Auctus.DataAccess.Asset
{
    public class AssetData : BaseSQL<DomainObjects.Asset.Asset>, IAssetData<DomainObjects.Asset.Asset>
    {
        private const string SQL_LIST_FOLLOWING_ASSETS = @"SELECT a.* FROM 
            [Asset] a
            INNER JOIN [FollowAsset] fa ON fa.AssetId = a.Id
            INNER JOIN [Follow] f ON f.Id = fa.Id
            INNER JOIN (SELECT f2.UserId, MAX(f2.CreationDate) CreationDate FROM [Follow] f2 GROUP BY f2.UserId) b 
                ON b.UserId = f.UserId AND f.CreationDate = b.CreationDate 
             WHERE f.ActionType = @ActionType
	            AND f.UserId = @UserId";

        public override string TableName => "Asset";

        public IEnumerable<DomainObjects.Asset.Asset> ListFollowingAssets(int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("ActionType", DomainObjects.Account.FollowActionType.Follow.Value, DbType.Int32);
            parameters.Add("UserId", userId, DbType.Int32);

            return Query<DomainObjects.Asset.Asset>(SQL_LIST_FOLLOWING_ASSETS, parameters);
        }
    }
}
