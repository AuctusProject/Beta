using Auctus.Business.Account;
using Auctus.Business.Advisor;
using Auctus.Business.Asset;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Service
{
    public abstract class BaseServices
    {
        protected readonly Cache MemoryCache;
        protected readonly ILoggerFactory Logger;

        protected BaseServices(ILoggerFactory loggerFactory, Cache cache)
        {
            MemoryCache = cache;
            Logger = loggerFactory;
        }

        protected UserBusiness UserBusiness { get { return new UserBusiness(Logger, MemoryCache); } }
        protected PasswordRecoveryBusiness PasswordRecoveryBusiness { get { return new PasswordRecoveryBusiness(Logger, MemoryCache); } }
        protected AdvisorBusiness AdvisorBusiness { get { return new AdvisorBusiness(Logger, MemoryCache); } }
        protected FollowBusiness FollowBusiness { get { return new FollowBusiness(Logger, MemoryCache); } }
        protected AssetBusiness AssetBusiness { get { return new AssetBusiness(Logger, MemoryCache); } }
        protected AssetValueBusiness AssetValueBusiness { get { return new AssetValueBusiness(Logger, MemoryCache); } }
        protected ExchangeApiAccessBusiness ExchangeApiAccessBusiness { get { return new ExchangeApiAccessBusiness(Logger, MemoryCache); } }
        protected RequestToBeAdvisorBusiness RequestToBeAdvisorBusiness { get { return new RequestToBeAdvisorBusiness(Logger, MemoryCache); } }
    }
}
