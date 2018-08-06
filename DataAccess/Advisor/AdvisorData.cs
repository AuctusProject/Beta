using Auctus.DataAccess.Core;
using Auctus.DomainObjects.Advisor;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Advisor
{
    public class AdvisorData : BaseSQL<DomainObjects.Advisor.Advisor>
    {
        public override string TableName => "Advisor";

        public List<DomainObjects.Advisor.Advisor> ListEnabled()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Enabled", true, DbType.Boolean);
            return SelectByParameters<DomainObjects.Advisor.Advisor>(parameters).ToList();
        }
    }
}
