using Auctus.DataAccess.Account;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Account
{
    public class ActionBusiness : BaseBusiness<Action, ActionData>
    {
        public ActionBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }
    }
}
