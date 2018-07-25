using Auctus.DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Account
{
    public class ActionData : BaseMongo<Action>
    {
        public override string CollectionName => "Action";
    }
}
