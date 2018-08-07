using Auctus.DataAccess.Core;
using Auctus.DomainObjects.Follow;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Follow
{
    public class FollowAssetData : BaseSQL<FollowAsset>
    {
        public override string TableName => "FollowAsset";

        private const string SQL_LIST = @"SELECT f.*, fa.AssetId FROM 
                                        [FollowAsset] fa
                                        INNER JOIN [Follow] f ON f.Id = fa.Id
                                        INNER JOIN (SELECT f2.UserId, MAX(f2.CreationDate) CreationDate FROM [Follow] f2 GROUP BY f2.UserId) b 
                                            ON b.UserId = f.UserId AND f.CreationDate = b.CreationDate 
                                         {0}";

        public List<FollowAsset> List(IEnumerable<int> assetsIds)
        {
            var complement = "";
            DynamicParameters parameters = new DynamicParameters();
            if (assetsIds?.Count() > 0)
            {
                complement = $"WHERE {string.Join(" OR ", assetsIds.Select((c, i) => $"fa.AssetId = @AssetId{i}"))}";
                for (int i = 0; i < assetsIds.Count(); ++i)
                    parameters.Add($"AssetId{i}", assetsIds.ElementAt(i), DbType.Int32);
            }
            return Query<FollowAsset>(string.Format(SQL_LIST, complement), parameters).ToList();
        }
    }
}
