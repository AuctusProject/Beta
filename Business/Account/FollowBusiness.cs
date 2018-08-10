using Auctus.DomainObjects.Account;
using Auctus.Util;
using DataAccessInterfaces.Account;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Account
{
    public class FollowBusiness : BaseBusiness<Follow, IFollowData<Follow>>
    {
        public FollowBusiness(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(serviceProvider, loggerFactory, cache, email, ip) { }

        public Follow Create(int userId, FollowActionType actionType)
        {
            return new Follow()
            {
                ActionType = actionType.Value,
                CreationDate = Data.GetDateTimeNow(),
                UserId = userId
            };
        }
    }
}
