using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class Login
    {
        public string Email { get; set; }
        public bool HasInvestment { get; set; }
        public bool IsAdvisor { get; set; }
        public bool PendingConfirmation { get; set; }
        public bool ResquestedToBeAdvisor { get; set; }
    }
}
