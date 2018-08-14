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
        public override string TableName => "Advisor";
        public AdvisorData(IConfigurationRoot configuration) : base(configuration) { }

        public List<DomainObjects.Advisor.Advisor> ListEnabled()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Enabled", true, DbType.Boolean);
            return SelectByParameters<DomainObjects.Advisor.Advisor>(parameters).ToList();
        }
    }
}
