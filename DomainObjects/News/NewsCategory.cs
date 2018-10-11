using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.News
{
    public class NewsCategory
    {
        [DapperKey(true)]
        [DapperType(System.Data.DbType.Int32)]
        public int Id { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int NewsId { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public string Description { get; set; }
    }
}
