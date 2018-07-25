using Auctus.DataAccess.Account;
using Auctus.DataAccess.Core;
using Auctus.DataAccess.Exchanges;
using Auctus.DomainObjects.Account;
using Auctus.Model;
using Auctus.Util;
using Auctus.Util.NotShared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Auctus.Business.Account
{
    public class UserBusiness : BaseBusiness<User, UserData>
    {
        public UserBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }
    }
}
