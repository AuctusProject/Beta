using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Account;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Account
{
    public class ActionData : BaseMongo<DomainObjects.Account.Action>, IActionData<DomainObjects.Account.Action>
    {
        public override string CollectionName => "Action";
        public ActionData(IConfigurationRoot configuration) : base(configuration) { }
    }
}
