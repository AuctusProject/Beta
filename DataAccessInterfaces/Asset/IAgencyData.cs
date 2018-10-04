using Auctus.DomainObjects.Asset;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessInterfaces.Asset
{
    public interface IAgencyData<T> : IBaseData<T>
    {
        List<Agency> ListAll();
    }
}
