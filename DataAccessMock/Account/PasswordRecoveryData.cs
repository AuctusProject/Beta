using System.Collections.Generic;
using System.Threading.Tasks;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;

namespace Auctus.DataAccessMock.Account
{
    public class PasswordRecoveryData : BaseData<PasswordRecovery>, IPasswordRecoveryData<PasswordRecovery>
    {
        public PasswordRecovery Get(int userId)
        {
            throw new System.NotImplementedException();
        }

        public PasswordRecovery Get(string token)
        {
            throw new System.NotImplementedException();
        }
    }
}
