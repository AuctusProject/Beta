using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Event
{
    public class LinkEventAsset
    {
        [DapperKey]
        [DapperType(System.Data.DbType.Int32)]
        public int AssetId { get; set; }
        [DapperKey]
        [DapperType(System.Data.DbType.Int32)]
        public int AssetEventId { get; set; }
    }
}
