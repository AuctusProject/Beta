using Auctus.Business.Account;
using Auctus.Business.Advisor;
using Auctus.Business.Asset;
using Auctus.DataAccess;
using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces;
using Auctus.DomainObjects.Account;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business
{
    public abstract class BaseBusiness<T, D> where D : IBaseData<T>
    {
        protected readonly IServiceProvider ServiceProvider;
        protected readonly ILoggerFactory LoggerFactory;
        protected readonly ILogger Logger;
        protected readonly Cache MemoryCache;
        protected readonly string LoggedEmail;
        protected readonly string LoggedIp;

        private D _data;
        protected D Data
        {
            get
            {
                if (_data == null)
                    _data = (D)ServiceProvider.GetService(typeof(D));
                return _data;
            }
        }

        private UserBusiness _userBusiness;
        private PasswordRecoveryBusiness _passwordRecoveryBusiness;
        private AdvisorBusiness _advisorBusiness;
        private AdviceBusiness _adviceBusiness;
        private AssetBusiness _assetBusiness;
        private AssetValueBusiness _assetValueBusiness;
        private FollowBusiness _followBusiness;
        private FollowAssetBusiness _followAssetBusiness;
        private FollowAdvisorBusiness _followAdvisorBusiness;
        private ExchangeApiAccessBusiness _exchangeApiAccessBusiness;
        private RequestToBeAdvisorBusiness _requestToBeAdvisorBusiness;
        private WalletBusiness _walletBusiness;
        private ActionBusiness _actionBusiness;

        protected BaseBusiness(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, Cache cache, string email, string ip)
        {
            ServiceProvider = serviceProvider;
            MemoryCache = cache;
            LoggerFactory = loggerFactory;
            Logger = loggerFactory.CreateLogger(GetType().Namespace);
            LoggedEmail = email;
            LoggedIp = ip;
        }

        public IEnumerable<T> ListAll()
        {
            return Data.SelectAll();
        }

        public void Insert(T obj)
        {
            Data.Insert(obj);
        }

        public void Update(T obj)
        {
            Data.Update(obj);
        }

        public void Delete(T obj)
        {
            Data.Delete(obj);
        }

        public User GetValidUser()
        {
            UserBusiness.BaseEmailValidation(LoggedEmail);
            var cacheKey = LoggedEmail.ToLower().Trim();
            var user = MemoryCache.Get<User>(cacheKey);
            if (user == null)
            {
                UserBusiness.EmailValidation(LoggedEmail);
                user = UserBusiness.GetByEmail(LoggedEmail);
                if (user == null)
                    throw new ArgumentException("User cannot be found.");
                if (!user.ConfirmationDate.HasValue)
                    throw new ArgumentException("Email was not confirmed.");

                if (!UserBusiness.IsValidAdvisor(user))
                {
                    WalletBusiness.ValidateUserWallet(user);
                    MemoryCache.Set<User>(cacheKey, user);
                }
                else
                    MemoryCache.Set<DomainObjects.Advisor.Advisor>(cacheKey, (DomainObjects.Advisor.Advisor)user);
                return user;
            }
            else
            {
                if (!UserBusiness.IsValidAdvisor(user))
                    WalletBusiness.ValidateUserWallet(user);
                else
                    user = MemoryCache.Get<DomainObjects.Advisor.Advisor>(cacheKey);
                return user;
            }
        }

        protected UserBusiness UserBusiness
        {
            get
            {
                if (_userBusiness == null)
                    _userBusiness = new UserBusiness(ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _userBusiness;
            }
        }

        protected PasswordRecoveryBusiness PasswordRecoveryBusiness
        {
            get
            {
                if (_passwordRecoveryBusiness == null)
                    _passwordRecoveryBusiness = new PasswordRecoveryBusiness(ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _passwordRecoveryBusiness;
            }
        }

        protected AdvisorBusiness AdvisorBusiness
        {
            get
            {
                if (_advisorBusiness == null)
                    _advisorBusiness = new AdvisorBusiness(ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _advisorBusiness;
            }
        }

        protected AdviceBusiness AdviceBusiness
        {
            get
            {
                if (_adviceBusiness == null)
                    _adviceBusiness = new AdviceBusiness(ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _adviceBusiness;
            }
        }

        protected FollowBusiness FollowBusiness
        {
            get
            {
                if (_followBusiness == null)
                    _followBusiness = new FollowBusiness(ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _followBusiness;
            }
        }

        protected FollowAssetBusiness FollowAssetBusiness
        {
            get
            {
                if (_followAssetBusiness == null)
                    _followAssetBusiness = new FollowAssetBusiness(ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _followAssetBusiness;
            }
        }

        protected FollowAdvisorBusiness FollowAdvisorBusiness
        {
            get
            {
                if (_followAdvisorBusiness == null)
                    _followAdvisorBusiness = new FollowAdvisorBusiness(ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _followAdvisorBusiness;
            }
        }

        protected AssetBusiness AssetBusiness
        {
            get
            {
                if (_assetBusiness == null)
                    _assetBusiness = new AssetBusiness(ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _assetBusiness;
            }
        }

        protected AssetValueBusiness AssetValueBusiness
        {
            get
            {
                if (_assetValueBusiness == null)
                    _assetValueBusiness = new AssetValueBusiness(ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _assetValueBusiness;
            }
        }

        protected ExchangeApiAccessBusiness ExchangeApiAccessBusiness
        {
            get
            {
                if (_exchangeApiAccessBusiness == null)
                    _exchangeApiAccessBusiness = new ExchangeApiAccessBusiness(ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _exchangeApiAccessBusiness;
            }
        }

        protected RequestToBeAdvisorBusiness RequestToBeAdvisorBusiness
        {
            get
            {
                if (_requestToBeAdvisorBusiness == null)
                    _requestToBeAdvisorBusiness = new RequestToBeAdvisorBusiness(ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _requestToBeAdvisorBusiness;
            }
        }

        protected WalletBusiness WalletBusiness
        {
            get
            {
                if (_walletBusiness == null)
                    _walletBusiness = new WalletBusiness(ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _walletBusiness;
            }
        }

        protected ActionBusiness ActionBusiness
        {
            get
            {
                if (_actionBusiness == null)
                    _actionBusiness = new ActionBusiness(ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _actionBusiness;
            }
        }
    }
}
