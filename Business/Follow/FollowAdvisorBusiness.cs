using Auctus.DataAccess.Core;
using Auctus.DataAccess.Follow;
using Auctus.DataAccessInterfaces.Follow;
using Auctus.DomainObjects.Follow;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Follow
{
    public class FollowAdvisorBusiness : BaseBusiness<FollowAdvisor, IFollowAdvisorData<FollowAdvisor>>
    {
        public FollowAdvisorBusiness(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(serviceProvider, loggerFactory, cache, email, ip) { }

        public List<FollowAdvisor> ListFollowers(IEnumerable<int> advisorIds = null)
        {
            return Data.ListFollowers(advisorIds);
        }

        public FollowAdvisor GetLastByUser(int userId, int advisorId)
        {
            return Data.GetLastByUserForAdvisor(userId, advisorId);
        }

        public void Create(int userId, int advisorId, FollowActionType actionType)
        {
            using (var transaction = new TransactionalDapperCommand())
            {
                var follow = FollowBusiness.Create(userId, actionType);
                transaction.Insert(follow);
                transaction.Insert(new FollowAdvisor()
                {
                    Id = follow.Id,
                    AdvisorId = advisorId
                });
                transaction.Commit();
            }
        }
    }
}
