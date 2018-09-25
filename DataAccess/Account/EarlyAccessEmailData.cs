using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Auctus.DataAccess.Account
{
    public class EarlyAccessEmailData : BaseSql<EarlyAccessEmail>, IEarlyAccessEmailData<EarlyAccessEmail>
    {
        public override string TableName => "EarlyAccessEmail";

        public EarlyAccessEmailData(IConfigurationRoot configuration) : base(configuration)
        {
        }
    }
}
