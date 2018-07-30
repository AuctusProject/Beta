using Auctus.DataAccess.Account;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Account
{
    public class ActionBusiness : BaseBusiness<DomainObjects.Account.Action, ActionData>
    {
        public ActionBusiness(ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(loggerFactory, cache, email, ip) { }

        public void InsertNewWallet(DateTime dateTime, int userId, string message, decimal? aucAmount)
        {
            Data.InsertOneAsync(new DomainObjects.Account.Action()
            {
                CreationDate = dateTime,
                UserId = userId,
                Type = 1,
                Message = message,
                AucAmount = aucAmount,
                Ip = LoggedIp
            });
        }

        public void InsertNewAucVerification(int userId, decimal aucAmount)
        {
            Data.InsertOneAsync(new DomainObjects.Account.Action()
            {
                CreationDate = DateTime.UtcNow,
                UserId = userId,
                Type = 2,
                AucAmount = aucAmount,
                Ip = LoggedIp
            });
        }
    }
}
