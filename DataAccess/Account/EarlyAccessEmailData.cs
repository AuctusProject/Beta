using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data;

namespace Auctus.DataAccess.Account
{
    public class EarlyAccessEmailData : BaseSql<EarlyAccessEmail>, IEarlyAccessEmailData<EarlyAccessEmail>
    {
        public override string TableName => "EarlyAccessEmail";

        public EarlyAccessEmailData(IConfigurationRoot configuration) : base(configuration)
        {
        }

        public EarlyAccessEmail GetByEmail(string email)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Email", email, DbType.AnsiString);
            return SelectByParameters<EarlyAccessEmail>(parameters).SingleOrDefault();
        }
    }
}
