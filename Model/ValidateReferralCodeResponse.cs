using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class ValidateReferralCodeResponse
    {
        public bool Valid { get; set; }
        public double Discount { get; set; }
        public double BonusAmount { get; set; }
    }
}
