using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Exchange;
using Auctus.DomainObjects.Exchange;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Exchange
{
    public class PairData : BaseSql<Pair>, IPairData<Pair>
    {
        public override string TableName => "Pair";

        public PairData(IConfigurationRoot configuration) : base(configuration) { }
    }
}
