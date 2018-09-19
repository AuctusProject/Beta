﻿using Auctus.DataAccess.Core;
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
                                                        {0}";

        public AssetCurrentValueData(IConfigurationRoot configuration) : base(configuration) { }

        public List<AssetCurrentValue> ListAllAssets(IEnumerable<int> ids)
        {
            var complement = "";
            DynamicParameters parameters = new DynamicParameters();
            if (ids?.Any() == true)
            {
                complement = $" WHERE ({string.Join(" OR ", ids.Select((c, i) => $"v.Id = @Id{i}"))})";
                for (int i = 0; i < ids.Count(); ++i)
                    parameters.Add($"Id{i}", ids.ElementAt(i), DbType.Int32);
            }
            return Query<AssetCurrentValue>(string.Format(SQL_LIST_ASSETS_VALUES, complement), parameters).ToList();
        }
    }
}