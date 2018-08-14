using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Asset;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Auctus.DataAccess.Asset
{
    public class AssetData : BaseSql<DomainObjects.Asset.Asset>, IAssetData<DomainObjects.Asset.Asset>
    {
        public override string TableName => "Asset";
        public AssetData(IConfigurationRoot configuration) : base(configuration) { }
    }
}
