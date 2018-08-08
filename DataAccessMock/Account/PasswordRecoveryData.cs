using System.Collections.Generic;
using System.Threading.Tasks;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;

namespace Auctus.DataAccessMock.Account
{
    public class PasswordRecoveryData : BaseData<PasswordRecovery>, IPasswordRecoveryData<PasswordRecovery>
    {
    }
}
