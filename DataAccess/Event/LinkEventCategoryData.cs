using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Event;
using Auctus.DomainObjects.Event;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Event
{
    public class LinkEventCategoryData : BaseSql<LinkEventCategory>, ILinkEventCategoryData<LinkEventCategory>
    {
        public override string TableName => "LinkEventCategory";

        public LinkEventCategoryData(IConfigurationRoot configuration) : base(configuration) { }
    }
}
