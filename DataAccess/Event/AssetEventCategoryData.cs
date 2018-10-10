using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Event;
using Auctus.DomainObjects.Event;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Event
{
    public class AssetEventCategoryData : BaseSql<AssetEventCategory>, IAssetEventCategoryData<AssetEventCategory>
    {
        public override string TableName => "AssetEventCategory";

        public AssetEventCategoryData(IConfigurationRoot configuration) : base(configuration) { }
    }
}
