using Auctus.DataAccessInterfaces.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Event
{
    public class AssetEventData<T> : BaseData<T>, IAssetEventData<T>
    {
    }
}
