using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Account
{
    public class PasswordRecoveryData : BaseSql<PasswordRecovery>, IPasswordRecoveryData<PasswordRecovery>
    {
        public override string TableName => "PasswordRecovery";
        public PasswordRecoveryData(IConfigurationRoot configuration) : base(configuration) { }

        public PasswordRecovery Get(int userId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("UserId", userId, DbType.Int32);
            return SelectByParameters<PasswordRecovery>(parameters).SingleOrDefault();
        }

        public PasswordRecovery Get(string token)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Token", token, DbType.AnsiString);
            return SelectByParameters<PasswordRecovery>(parameters).SingleOrDefault();
        }
    }
}
