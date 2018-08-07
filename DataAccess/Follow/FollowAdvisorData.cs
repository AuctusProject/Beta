using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Follow;
using Auctus.DomainObjects.Follow;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Auctus.DataAccess.Follow
{
    public class FollowAdvisorData : BaseSQL<FollowAdvisor>, IFollowAdvisorData<FollowAdvisor>
    {
        public override string TableName => "FollowAdvisor";

        private const string SQL_LIST = @"SELECT f.*, fa.AdvisorId FROM 
                                        [FollowAdvisor] fa
                                        INNER JOIN [Follow] f ON f.Id = fa.Id
                                        INNER JOIN (SELECT f2.UserId, MAX(f2.CreationDate) CreationDate FROM [Follow] f2 GROUP BY f2.UserId) b 
                                            ON b.UserId = f.UserId AND f.CreationDate = b.CreationDate 
                                         WHERE {0}";

        private const string SQL_GET_LAST_BY_USER = @"SELECT f.*, fa.AdvisorId FROM 
                                        [FollowAdvisor] fa
                                        INNER JOIN [Follow] f ON f.Id = fa.Id
                                        INNER JOIN (SELECT f2.UserId, MAX(f2.CreationDate) CreationDate FROM [Follow] f2 GROUP BY f2.UserId) b 
                                            ON b.UserId = f.UserId AND f.CreationDate = b.CreationDate 
                                         WHERE f.UserId = @UserId
                                            AND fa.AdvisorId = @AdvisorId";

        public List<FollowAdvisor> List(IEnumerable<int> advisorIds)
        {
            var complement = "";
            DynamicParameters parameters = new DynamicParameters();
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
