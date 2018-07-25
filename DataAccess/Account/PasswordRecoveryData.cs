using Auctus.DomainObjects.Account;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Account
{
    public class PasswordRecoveryData : BaseSQL<PasswordRecovery>
    {
        public override string TableName => "PasswordRecovery";
    }
}
