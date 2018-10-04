using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Asset;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Asset
{
    public class AgencyRatingData : BaseSql<AgencyRating>, IAgencyRatingData<AgencyRating>
    {
        public override string TableName => "AgencyRating";

        public AgencyRatingData(IConfigurationRoot configuration) : base(configuration) { }
    }
}
