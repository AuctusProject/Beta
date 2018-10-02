using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Asset;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Asset
{
    public class AgencyData : BaseData<Agency>, IAgencyData<Agency>
    {
        public List<Agency> ListAll()
        {
            return new List<Agency>();
        }
    }
}
