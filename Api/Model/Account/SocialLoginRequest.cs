using Auctus.DomainObjects.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Model.Account
{
    public class SocialLoginRequest
    {
        public int SocialNetworkType { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public bool RequestedToBeAdvisor { get; set; }
    }
}
