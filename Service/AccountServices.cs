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
        public AccountServices(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }
    }
}
