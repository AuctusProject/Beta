using Auctus.DataAccess.Core;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Auctus.DataAccessInterfaces.Advisor;

namespace Auctus.DataAccess.Advisor
{
    public class FollowAdvisorData : BaseSql<FollowAdvisor>, IFollowAdvisorData<FollowAdvisor>
    {
        public override string TableName => "FollowAdvisor";
        public FollowAdvisorData(IConfigurationRoot configuration) : base(configuration) { }

        private const string SQL_LIST = @"
		SELECT 
			f.*, fa.AdvisorId {0}
		FROM 
		    [FollowAdvisor] fa WITH(NOLOCK)
		    INNER JOIN [Follow] f WITH(NOLOCK) ON f.Id = fa.Id
            {1}
		    INNER JOIN (
		    	SELECT f2.UserId, MAX(f2.CreationDate) CreationDate, fa2.AdvisorId
		    	FROM 
		    		[FollowAdvisor] fa2 WITH(NOLOCK)
		    		INNER JOIN [Follow] f2 WITH(NOLOCK) ON f2.Id = fa2.Id
		    	GROUP BY f2.UserId, fa2.AdvisorId) b 
			ON b.UserId = f.UserId AND f.CreationDate = b.CreationDate AND b.AdvisorId = fa.AdvisorId
		WHERE 
			f.ActionType = @ActionType AND ({2})";

        private const string SQL_GET_LAST_BY_USER = @"
		SELECT 
			f.*, fa.AdvisorId 
		FROM 
		    [FollowAdvisor] fa WITH(NOLOCK)
		    INNER JOIN [Follow] f WITH(NOLOCK) ON f.Id = fa.Id
		    INNER JOIN (
		    	SELECT f2.UserId, MAX(f2.CreationDate) CreationDate, fa2.AdvisorId
		    	FROM 
		    		[FollowAdvisor] fa2 WITH(NOLOCK)
		    		INNER JOIN [Follow] f2 WITH(NOLOCK) ON f2.Id = fa2.Id
		    	GROUP BY f2.UserId, fa2.AdvisorId) b 
			ON b.UserId = f.UserId AND f.CreationDate = b.CreationDate AND b.AdvisorId = fa.AdvisorId
		WHERE 
			f.UserId = @UserId
		    AND fa.AdvisorId = @AdvisorId";

        private const string SQL_LIST_ADVISORS_FOLLOWED = @"
		SELECT 
			f.*, fa.AdvisorId 
		FROM 
		    [FollowAdvisor] fa WITH(NOLOCK)
		    INNER JOIN [Follow] f WITH(NOLOCK) ON f.Id = fa.Id
		    INNER JOIN (
		    	SELECT f2.UserId, MAX(f2.CreationDate) CreationDate, fa2.AdvisorId
		    	FROM 
		    		[FollowAdvisor] fa2 WITH(NOLOCK)
		    		INNER JOIN [Follow] f2 WITH(NOLOCK) ON f2.Id = fa2.Id
                WHERE f2.UserId = @UserId
		    	GROUP BY f2.UserId, fa2.AdvisorId) b 
			ON b.UserId = f.UserId AND f.CreationDate = b.CreationDate AND b.AdvisorId = fa.AdvisorId
		WHERE 
			f.ActionType = @ActionType AND f.UserId = @UserId";

        private const string SQL_COUNT_ADVISOR_FOLLOWERS = @"
		SELECT 
			count(f.Id)
        FROM 
		    [FollowAdvisor] fa WITH(NOLOCK)
		    INNER JOIN [Follow] f WITH(NOLOCK) ON f.Id = fa.Id
		    INNER JOIN (
		    	SELECT f2.UserId, MAX(f2.CreationDate) CreationDate, fa2.AdvisorId
		    	FROM 
		    		[FollowAdvisor] fa2 WITH(NOLOCK)
		    		INNER JOIN [Follow] f2 WITH(NOLOCK) ON f2.Id = fa2.Id
                WHERE fa2.AdvisorId = @AdvisorId
		    	GROUP BY f2.UserId, fa2.AdvisorId) b 
			ON b.UserId = f.UserId AND f.CreationDate = b.CreationDate AND b.AdvisorId = fa.AdvisorId
		WHERE 
			f.ActionType = @ActionType AND fa.AdvisorId = @AdvisorId";

        public List<FollowAdvisor> ListFollowers(IEnumerable<int> advisorIds, bool includeUserData)
        {
            var complement = "";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ActionType", FollowActionType.Follow.Value, DbType.Int32);
            if (advisorIds.Count() > 0)
            {
                complement = string.Join(" OR ", advisorIds.Select((c, i) => $"fa.AdvisorId = @AdvisorId{i}"));
                for (int i = 0; i < advisorIds.Count(); ++i)
                    parameters.Add($"AdvisorId{i}", advisorIds.ElementAt(i), DbType.Int32);
            }
            if (includeUserData)
            {
                return Query<FollowAdvisor, User, FollowAdvisor>(string.Format(SQL_LIST, ", u.*", " INNER JOIN [User] u ON u.Id = f.UserId ", complement),
                    (follow, user) =>
                    {
                        follow.User = user;
                        return follow;
                    }, "Id", parameters).ToList();
            }
            else
                return Query<FollowAdvisor>(string.Format(SQL_LIST, "", "", complement), parameters).ToList();
        }

        public FollowAdvisor GetLastByUserForAdvisor(int userId, int advisorId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId, DbType.Int32);
            parameters.Add("AdvisorId", advisorId, DbType.Int32);
            return Query<FollowAdvisor>(SQL_GET_LAST_BY_USER, parameters).SingleOrDefault();
        }

        public List<FollowAdvisor> ListAdvisorsFollowed(int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId, DbType.Int32);
            parameters.Add("ActionType", FollowActionType.Follow.Value, DbType.Int32);
            return Query<FollowAdvisor>(SQL_LIST_ADVISORS_FOLLOWED, parameters).ToList();
        }

        public int CountAdvisorFollowers(int advisorId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ActionType", FollowActionType.Follow.Value, DbType.Int32);
            parameters.Add("AdvisorId", advisorId, DbType.Int32);
            return Query<int?>(SQL_COUNT_ADVISOR_FOLLOWERS, parameters).FirstOrDefault() ?? 0;
        }
    }
}
