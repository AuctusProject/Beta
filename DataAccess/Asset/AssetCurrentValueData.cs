using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Asset;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Asset
{
    public class AssetCurrentValueData : BaseSql<DomainObjects.Asset.AssetCurrentValue>, IAssetCurrentValueData<DomainObjects.Asset.AssetCurrentValue>
    {
        public override string TableName => "AssetCurrentValue";

        public AssetCurrentValueData(IConfigurationRoot configuration) : base(configuration) { }
    }
}
