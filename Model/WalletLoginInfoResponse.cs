using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class WalletLoginInfoResponse : SetReferralCodeResponse
    {
        public string ReferralCode { get; set; }
        public string RegisteredWallet { get; set; }
    }
}
