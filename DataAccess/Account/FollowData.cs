﻿using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Account
{
    public class FollowData : BaseSql<Follow>, IFollowData<Follow>
    {
        public override string TableName => "Follow";
        public FollowData(IConfigurationRoot configuration) : base(configuration) { }
    }
}
