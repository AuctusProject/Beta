using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Account
{
    public class Action : MongoDomainObject
    {
        public DateTime CreationDate { get; set; }
        public string Ip { get; set; }
        public int UserId { get; set; }
        public decimal? AucAmount { get; set; }
        public int Type { get; set; }
        public string Message { get; set; }
    }
}
