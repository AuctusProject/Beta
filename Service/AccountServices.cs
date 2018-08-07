using Auctus.DomainObjects.Account;
using Auctus.Model;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.Service
{
    public class AccountServices : BaseServices
    {
        public AccountServices(ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(loggerFactory, cache, email, ip) { }

        public LoginResponse ValidateSignature(string address, string signature)
        {
            return UserBusiness.ValidateSignature(address, signature);
        }

        public LoginResponse Login(string email, string password)
        {
            return UserBusiness.Login(email, password);
        }

        public async Task<LoginResponse> Register(string email, string password, bool requestedToBeAdvisor)
        {
            return await UserBusiness.Register(email, password, requestedToBeAdvisor);
        }
    }
}
