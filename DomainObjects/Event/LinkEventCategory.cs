using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Event
{
    public class LinkEventCategory
    {
        [DapperKey]
        [DapperType(System.Data.DbType.Int32)]
        public int AssetEventId { get; set; }
        [DapperKey]
        [DapperType(System.Data.DbType.Int32)]
        public int AssetEventCategoryId { get; set; }
    }
}
