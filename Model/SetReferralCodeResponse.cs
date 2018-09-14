using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class SetReferralCodeResponse
    {
        public bool Valid { get; set; }
        public decimal AUCRequired { get; set; }
        public double Discount { get; set; }
        public int StandardAUCAmount { get; set; }
    }
}
