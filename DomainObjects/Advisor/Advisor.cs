using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Advisor
{
    public class Advisor
    {
        [DapperKey]
        [DapperType(System.Data.DbType.AnsiString)]
        public int UserId { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public string Name { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public string Description { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public DateTime CreationDate { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public string UrlPhoto { get; set; }
    }
}
