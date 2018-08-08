using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public bool HasInvestment { get; set; }
        public bool IsAdvisor { get; set; }
        public bool PendingConfirmation { get; set; }
        public bool RequestedToBeAdvisor { get; set; }
    }
}
