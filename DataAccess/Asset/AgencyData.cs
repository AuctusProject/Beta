using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Asset;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Asset
{
    public class AgencyData : BaseSql<Agency>, IAgencyData<Agency>
    {
        public override string TableName => "Agency";

        private const string SQL_LIST = @"SELECT a.*, r.* FROM 
                                        [Agency] a WITH(NOLOCK)
                                        LEFT JOIN [AgencyRating] r WITH(NOLOCK) ON r.AgencyId = a.Id";

        public AgencyData(IConfigurationRoot configuration) : base(configuration) { }

        public List<Agency> ListAll()
        {
            return QueryParentChild<Agency, AgencyRating, int>(SQL_LIST, c => c.Id, c => c.AgencyRating, "Id").ToList();
        }
    }
}
