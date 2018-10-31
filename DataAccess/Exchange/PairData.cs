using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Exchange;
using Auctus.DomainObjects.Exchange;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Auctus.DataAccess.Exchange
{
    public class PairData : BaseSql<Pair>, IPairData<Pair>
    {
        public override string TableName => "Pair";

        public PairData(IConfigurationRoot configuration) : base(configuration) { }

        public IEnumerable<Pair> ListEnabled()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Enabled", true, DbType.Boolean);
            return SelectByParameters<Pair>(parameters);
        }
    }
}
