using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Account
{
    public class FollowBusiness : BaseBusiness<Follow, IFollowData<Follow>>
    {
        public FollowBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

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
