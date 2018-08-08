using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Account
{
    public class ActionData : BaseData<DomainObjects.Account.Action>, IActionData<DomainObjects.Account.Action>
    {
    }
}
