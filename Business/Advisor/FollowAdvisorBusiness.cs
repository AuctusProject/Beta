using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auctus.Business.Advisor
{
    public class FollowAdvisorBusiness : BaseBusiness<FollowAdvisor, IFollowAdvisorData<FollowAdvisor>>
    {
        public FollowAdvisorBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public List<FollowAdvisor> ListFollowers(IEnumerable<int> advisorIds, bool includeUserData)
        {
            return Data.ListFollowers(advisorIds, includeUserData);
        }

        public FollowAdvisor GetLastByUser(int userId, int advisorId)
        {
            return Data.GetLastByUserForAdvisor(userId, advisorId);
        }

        public List<int> ListAdvisorsFollowed(int userId)
        {
            return Data.ListAdvisorsFollowed(userId).Select(c => c.AdvisorId).ToList();
        }

        public FollowAdvisor Create(int userId, int advisorId, FollowActionType actionType)
        {
            using (var transaction = TransactionalDapperCommand)
            {
                var follow = FollowBusiness.Create(userId, actionType);
                transaction.Insert(follow);
                FollowAdvisor followAdvisor = new FollowAdvisor()
                {
                    Id = follow.Id,
                    AdvisorId = advisorId,
                    ActionType = follow.ActionType,
                    UserId = follow.UserId,
                    CreationDate = follow.CreationDate
                };
                transaction.Insert(followAdvisor);
                transaction.Commit();

                return followAdvisor;
            }
        }
    }
}
