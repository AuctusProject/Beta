using Auctus.DataAccessInterfaces.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Event
{
    public class LinkEventAssetData<T> : BaseData<T>, ILinkEventAssetData<T>
    {
    }
}
