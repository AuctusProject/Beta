using Auctus.DataAccess.Core;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Follow;
using Auctus.DataAccess.Follow;
using FollowObj = Auctus.DomainObjects.Follow.Follow;
using Auctus.DataAccessInterfaces.Follow;

namespace Auctus.Business.Follow
{
    public class FollowBusiness : BaseBusiness<DomainObjects.Follow.Follow, IFollowData<DomainObjects.Follow.Follow>>
    {
        public FollowBusiness(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(serviceProvider, loggerFactory, cache, email, ip) { }

        public FollowObj Create(int userId, FollowActionType actionType)
        {
            return new FollowObj()
            {
                ActionType = actionType.Value,
                CreationDate = DateTime.UtcNow,
                UserId = userId
            };
        }
    }
}
