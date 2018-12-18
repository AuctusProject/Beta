using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Asset;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Asset
{
    public class AssetCurrentValueData : BaseSql<AssetCurrentValue>, IAssetCurrentValueData<AssetCurrentValue>
    {
        public override string TableName => "AssetCurrentValue";

        private const string SQL_LIST_ASSETS_VALUES = @"SELECT v.*, a.* FROM 
                                                        [AssetCurrentValue] v WITH(NOLOCK)
                                                        INNER JOIN [Asset] a WITH(NOLOCK) ON a.Id = v.Id
                                                        WHERE a.Enabled = @Enabled {0}";

        private const string SQL_LIST_FOLLOWING_ASSETS = @"
        SELECT 
            v.*, a.* 
        FROM 
            [AssetCurrentValue] v WITH(NOLOCK)
            INNER JOIN [Asset] a WITH(NOLOCK) ON a.Id = v.Id
            INNER JOIN [FollowAsset] fa WITH(NOLOCK) ON fa.AssetId = a.Id
            INNER JOIN [Follow] f WITH(NOLOCK) ON f.Id = fa.Id
            INNER JOIN (
				SELECT 
                    f2.UserId, fa2.AssetId, MAX(f2.CreationDate) CreationDate 
				FROM 
                    [FollowAsset] fa2 WITH(NOLOCK)
				    INNER JOIN [Follow] f2 WITH(NOLOCK) ON f2.Id = fa2.Id 
				GROUP BY f2.UserId, fa2.AssetId) b 
		    ON b.UserId = f.UserId AND f.CreationDate = b.CreationDate AND b.AssetId = fa.AssetId 
        WHERE 
            a.Enabled = @Enabled 
            AND f.ActionType = @ActionType
	        AND f.UserId = @UserId ";

        public AssetCurrentValueData(IConfigurationRoot configuration) : base(configuration) { }

        public List<AssetCurrentValue> ListAllAssets(bool enabled, IEnumerable<int> ids)
        {
            var complement = "";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Enabled", enabled, DbType.Boolean);
            if (ids?.Any() == true)
            {
                complement = $" AND ({string.Join(" OR ", ids.Select((c, i) => $"v.Id = @Id{i}"))})";
                for (int i = 0; i < ids.Count(); ++i)
                    parameters.Add($"Id{i}", ids.ElementAt(i), DbType.Int32);
            }
            return Query<AssetCurrentValue>(string.Format(SQL_LIST_ASSETS_VALUES, complement), parameters).ToList();
        }

        public List<AssetCurrentValue> ListAssetsFollowedByUser(int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Enabled", true, DbType.Boolean);
            parameters.Add("ActionType", DomainObjects.Account.FollowActionType.Follow.Value, DbType.Int32);
            parameters.Add("UserId", userId, DbType.Int32);

            return Query<AssetCurrentValue>(SQL_LIST_FOLLOWING_ASSETS, parameters).ToList();
        }

        public void UpdateAssetValue(IEnumerable<AssetCurrentValue> assetCurrentValues)
        {
            if (assetCurrentValues == null || !assetCurrentValues.Any())
                return;

            var updateSql = "";
            foreach (var value in assetCurrentValues)
            {
                updateSql += $"UPDATE [AssetCurrentValue] SET UpdateDate = {GetDateTimeSqlFormattedValue(value.UpdateDate)}, CurrentValue = {GetDoubleSqlFormattedValue(value.CurrentValue)}, " +
                            $"Variation24Hours = {GetDoubleSqlFormattedValue(value.Variation24Hours)}, BidValue = {GetDoubleSqlFormattedValue(value.BidValue)}, " +
                            $"AskValue = {GetDoubleSqlFormattedValue(value.AskValue)} WHERE Id = {value.Id};";
            }
            Execute(updateSql, null, 240);
        }

        public void UpdateAssetValue7And30Days(IEnumerable<AssetCurrentValue> assetCurrentValues)
        {
            if (assetCurrentValues == null || !assetCurrentValues.Any())
                return;

            var updateSql = "";
            foreach (var value in assetCurrentValues)
            {
                updateSql += $"UPDATE [AssetCurrentValue] SET Variation7Days = {GetDoubleSqlFormattedValue(value.Variation7Days)}, " +
                            $"Variation30Days = {GetDoubleSqlFormattedValue(value.Variation30Days)} WHERE Id = {value.Id};";
            }
            Execute(updateSql, null, 240);
        }
    }
}
