using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Event
{
    public class AssetEventCategory
    {
        [DapperType(System.Data.DbType.Int32)]
        public int Id { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public string Name { get; set; }
    }
}
