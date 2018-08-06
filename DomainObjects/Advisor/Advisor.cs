using Auctus.DomainObjects.Account;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Advisor
{
    public class Advisor : User
    {
        [DapperType(System.Data.DbType.AnsiString)]
        public string Name { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public string Description { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public DateTime BecameAdvisorDate { get; set; }
        [DapperType(System.Data.DbType.Boolean)]
        public bool Enabled { get; set; }

        //public List<Advice> Advice { get; set; } = new List<Advice>();
    }
}
