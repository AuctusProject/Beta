using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.News
{
    public class News
    {
        [DapperKey(true)]
        [DapperType(System.Data.DbType.Int32)]
        public int Id { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public string Title { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public string Description { get; set; }
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime ExternalCreationDate { get; set; }
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime CreationDate { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public string Link { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int SourceId { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public string ExternalId { get; set; }

        public List<NewsCategory> NewsCategory { get; set; } = new List<NewsCategory>();
    }
}
