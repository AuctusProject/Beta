using Auctus.Business.Account;
using Auctus.Business.Advisor;
using Auctus.Business.Asset;
using Auctus.Business.Follow;
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
        protected readonly string LoggedEmail;
        protected readonly string LoggedIp;

        protected BaseServices(ILoggerFactory loggerFactory, Cache cache, string email, string ip)
        {
            MemoryCache = cache;
            Logger = loggerFactory;
            LoggedEmail = email;
            LoggedIp = ip;
        }

        protected UserBusiness UserBusiness { get { return new UserBusiness(Logger, MemoryCache, LoggedEmail, LoggedIp); } }
        protected WalletBusiness WalletBusiness { get { return new WalletBusiness(Logger, MemoryCache, LoggedEmail, LoggedIp); } }
        protected ActionBusiness ActionBusiness { get { return new ActionBusiness(Logger, MemoryCache, LoggedEmail, LoggedIp); } }
        protected PasswordRecoveryBusiness PasswordRecoveryBusiness { get { return new PasswordRecoveryBusiness(Logger, MemoryCache, LoggedEmail, LoggedIp); } }
        protected AdvisorBusiness AdvisorBusiness { get { return new AdvisorBusiness(Logger, MemoryCache, LoggedEmail, LoggedIp); } }
        protected AdviceBusiness AdviceBusiness { get { return new AdviceBusiness(Logger, MemoryCache, LoggedEmail, LoggedIp); } }
        protected FollowBusiness FollowBusiness { get { return new FollowBusiness(Logger, MemoryCache, LoggedEmail, LoggedIp); } }
        protected FollowAssetBusiness FollowAssetBusiness { get { return new FollowAssetBusiness(Logger, MemoryCache, LoggedEmail, LoggedIp); } }
        protected FollowAdvisorBusiness FollowAdvisorBusiness { get { return new FollowAdvisorBusiness(Logger, MemoryCache, LoggedEmail, LoggedIp); } }
        protected AssetBusiness AssetBusiness { get { return new AssetBusiness(Logger, MemoryCache, LoggedEmail, LoggedIp); } }
        protected AssetValueBusiness AssetValueBusiness { get { return new AssetValueBusiness(Logger, MemoryCache, LoggedEmail, LoggedIp); } }
        protected ExchangeApiAccessBusiness ExchangeApiAccessBusiness { get { return new ExchangeApiAccessBusiness(Logger, MemoryCache, LoggedEmail, LoggedIp); } }
        protected RequestToBeAdvisorBusiness RequestToBeAdvisorBusiness { get { return new RequestToBeAdvisorBusiness(Logger, MemoryCache, LoggedEmail, LoggedIp); } }
    }
}
