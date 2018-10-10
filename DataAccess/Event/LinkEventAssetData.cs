using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Event;
using Auctus.DomainObjects.Event;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Event
{
    public class LinkEventAssetData : BaseSql<LinkEventAsset>, ILinkEventAssetData<LinkEventAsset>
    {
        public override string TableName => "LinkEventAsset";

        public LinkEventAssetData(IConfigurationRoot configuration) : base(configuration) { }
    }
}
