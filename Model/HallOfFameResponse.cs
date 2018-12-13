using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class HallOfFameResponse
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<AdvisorResponse> Advisors { get; set; } = new List<AdvisorResponse>();
    }
}
