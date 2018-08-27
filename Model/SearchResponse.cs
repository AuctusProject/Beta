using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class SearchResponse
    {
        public List<AssetResult> Assets { get; set; }
        public List<AdvisorResult> Advisors { get; set; }

        public class AssetResult
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public bool HasAdvice { get; set; }
        }

        public class AdvisorResult
        {
            public int UserId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public bool Enabled { get; set; }
        }

        public SearchResponse()
        {
            Assets = new List<AssetResult>();
            Advisors = new List<AdvisorResult>();
        }
    }
}
