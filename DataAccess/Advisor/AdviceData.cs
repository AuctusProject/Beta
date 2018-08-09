using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Advisor;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Advisor
{
    public class AdviceData : BaseSQL<Advice>, IAdviceData<Advice>
    {
        public override string TableName => "Advice";

        private const string SQL_LIST = @"SELECT a.* FROM [Advice] a WHERE {0}";

        public List<Advice> List(IEnumerable<int> advisorIds)
        {
            var complement = "";
            DynamicParameters parameters = new DynamicParameters();
            if (advisorIds.Count() > 0)
            {
                complement = string.Join(" OR ", advisorIds.Select((c, i) => $"a.AdvisorId = @AdvisorId{i}"));
                for (int i = 0; i < advisorIds.Count(); ++i)
                    parameters.Add($"AdvisorId{i}", advisorIds.ElementAt(i), DbType.Int32);
            }
            return Query<Advice>(string.Format(SQL_LIST, complement), parameters).ToList();
        }
    }
}
