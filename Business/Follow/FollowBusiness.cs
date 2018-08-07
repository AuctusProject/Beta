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

namespace Auctus.Business.Follow
{
    public class FollowBusiness : BaseBusiness<Auctus.DomainObjects.Follow.Follow, FollowData>
    {
        public FollowBusiness(ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(loggerFactory, cache, email, ip) { }

        public FollowObj Create(int userId, FollowActionType actionType)
        {
            var follow = new FollowObj();
            follow.ActionType = actionType.Value;
            follow.CreationDate = DateTime.UtcNow;
            follow.UserId = userId;

            Data.Insert(follow);

            return follow;
        }
    }
}
