using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Asset;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Asset
{
    public class AgencyData : BaseSql<Agency>, IAgencyData<Agency>
    {
        public override string TableName => "Agency";

        public AgencyData(IConfigurationRoot configuration) : base(configuration) { }
    }
}
