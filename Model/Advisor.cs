using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class Advisor
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public string UrlPhoto { get; set; }
    }
}
