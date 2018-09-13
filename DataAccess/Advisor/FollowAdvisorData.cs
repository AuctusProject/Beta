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
			f.*, fa.AdvisorId 
		FROM 
		    [FollowAdvisor] fa
		    INNER JOIN [Follow] f ON f.Id = fa.Id
		    INNER JOIN (
		    	SELECT f2.UserId, MAX(f2.CreationDate) CreationDate, fa2.AdvisorId
		    	FROM 
		    		[FollowAdvisor] fa2
		    		INNER JOIN [Follow] f2 ON f2.Id = fa2.Id
		    	GROUP BY f2.UserId, fa2.AdvisorId) b 
			ON b.UserId = f.UserId AND f.CreationDate = b.CreationDate AND b.AdvisorId = fa.AdvisorId
		WHERE 
			f.ActionType = @ActionType AND ({0})";

        private const string SQL_GET_LAST_BY_USER = @"
		SELECT 
			f.*, fa.AdvisorId 
		FROM 
		    [FollowAdvisor] fa
		    INNER JOIN [Follow] f ON f.Id = fa.Id
		    INNER JOIN (
		    	SELECT f2.UserId, MAX(f2.CreationDate) CreationDate, fa2.AdvisorId
		    	FROM 
		    		[FollowAdvisor] fa2
		    		INNER JOIN [Follow] f2 ON f2.Id = fa2.Id
		    	GROUP BY f2.UserId, fa2.AdvisorId) b 
			ON b.UserId = f.UserId AND f.CreationDate = b.CreationDate AND b.AdvisorId = fa.AdvisorId
		WHERE 
			f.UserId = @UserId
		    AND fa.AdvisorId = @AdvisorId";

        public List<FollowAdvisor> ListFollowers(IEnumerable<int> advisorIds)
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
            return Query<FollowAdvisor>(string.Format(SQL_LIST, complement), parameters).ToList();
        }

        public FollowAdvisor GetLastByUserForAdvisor(int userId, int advisorId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId, DbType.Int32);
            parameters.Add("AdvisorId", advisorId, DbType.Int32);

            return Query<FollowAdvisor>(SQL_GET_LAST_BY_USER, parameters).SingleOrDefault();
        }
    }
}
