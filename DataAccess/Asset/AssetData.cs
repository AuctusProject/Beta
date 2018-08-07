using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Asset;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Auctus.DataAccess.Asset
{
    public class AssetData : BaseSQL<DomainObjects.Asset.Asset>, IAssetData<DomainObjects.Asset.Asset>
    {
        public override string TableName => "Asset";
    }
}
