using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Event;
using Auctus.DomainObjects.Event;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Event
{
    public class AssetEventData : BaseSql<AssetEvent>, IAssetEventData<AssetEvent>
    {
        public override string TableName => "AssetEvent";

        public AssetEventData(IConfigurationRoot configuration) : base(configuration) { }
    }
}
