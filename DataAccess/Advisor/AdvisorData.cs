using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Advisor
{
    public class AdvisorData : BaseSql<DomainObjects.Advisor.Advisor>, IAdvisorData<DomainObjects.Advisor.Advisor>
    {
        private const string SQL_LIST_FOLLOWING_ADVISORS = @"SELECT a.* FROM 
            [Advisor] a
            INNER JOIN [FollowAdvisor] fa ON fa.AdvisorId = a.Id
            INNER JOIN [Follow] f ON f.Id = fa.Id
            INNER JOIN (
		    	SELECT f2.UserId, MAX(f2.CreationDate) CreationDate, fa2.AdvisorId
		    	FROM 
		    		[FollowAdvisor] fa2
		    		INNER JOIN [Follow] f2 ON f2.Id = fa2.Id
		    	GROUP BY f2.UserId, fa2.AdvisorId) b 
			ON b.UserId = f.UserId AND f.CreationDate = b.CreationDate AND b.AdvisorId = fa.AdvisorId
             WHERE f.ActionType = @ActionType
	            AND f.UserId = @UserId";

        private const string SQL_GET_BY_ID = @"SELECT * FROM 
            [Advisor] a 
            INNER JOIN [User] u ON a.Id = u.Id 
            WHERE u.Id = @Id";

        public override string TableName => "Advisor";
        public AdvisorData(IConfigurationRoot configuration) : base(configuration) { }

        public List<DomainObjects.Advisor.Advisor> ListEnabled()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Enabled", true, DbType.Boolean);
            return SelectByParameters<DomainObjects.Advisor.Advisor>(parameters).ToList();
        }

        public IEnumerable<DomainObjects.Advisor.Advisor> ListFollowingAdvisors(int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("ActionType", DomainObjects.Account.FollowActionType.Follow.Value, DbType.Int32);
            parameters.Add("UserId", userId, DbType.Int32);

            return Query<DomainObjects.Advisor.Advisor>(SQL_LIST_FOLLOWING_ADVISORS, parameters);
        }

        public DomainObjects.Advisor.Advisor GetAdvisor(int id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);
            return Query<DomainObjects.Advisor.Advisor>(SQL_GET_BY_ID, parameters).FirstOrDefault();
        }
    }
}
