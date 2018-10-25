using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class ReferralProgramInfoResponse
    {
        public string ReferralCode { get; set; }
        public double Pending { get; set; }   
        public double Canceled { get; set; }
        public double CashedOut { get; set; }
        public double Available { get; set; }
        public double BonusToReferred { get; set; }
    }
}
