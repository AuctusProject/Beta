using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class ReferralProgramInfoResponse
    {
        public string ReferralCode { get; set; }
        public int InProgressCount { get; set; }   
        public int InterruptedCount { get; set; }
        public int FinishedCount { get; set; }
        public int PaidCount { get; set; }
        public int NotStartedCount { get; set; }
    }
}
