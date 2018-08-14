using Auctus.DataAccess.Core;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Asset;
using Dapper;
using DataAccessInterfaces.Asset;
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

        private const string SQL_LIST = @"SELECT f.*, fa.AssetId FROM 
                                        [FollowAsset] fa
                                        INNER JOIN [Follow] f ON f.Id = fa.Id
                                        INNER JOIN (SELECT f2.UserId, MAX(f2.CreationDate) CreationDate FROM [Follow] f2 GROUP BY f2.UserId) b 
                                            ON b.UserId = f.UserId AND f.CreationDate = b.CreationDate 
                                        WHERE f.ActionType = @ActionType {0}";

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
    }
}
