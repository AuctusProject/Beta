using Auctus.Business.Account;
using Auctus.Business.Advisor;
using Auctus.Business.Asset;
using Auctus.Business.Blockchain;
using Auctus.DataAccess;
using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces;
using Auctus.DomainObjects.Account;
using Auctus.Util;
using Auctus.Util.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business
{
    public abstract class BaseBusiness<T, D> where D : IBaseData<T>
    {
        protected readonly IConfigurationRoot Configuration;
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
        private Web3Business _web3Business;

        private string _apiUrl;
        private string _webUrl;
        private string _web3Url;
        private string _web3Route;
        private int? _minimumAucLogin;
        private int? _minimumTimeInSecondsBetweenAdvices;
        private List<string> _emailErrorList;
        private string _storageConfiguration;
        private string _sendGridKey;
        private string _hashSecret;

        protected BaseBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, ILoggerFactory loggerFactory, Cache cache, string email, string ip)
        {
            Configuration = configuration;
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
                    throw new NotFoundException("User cannot be found.");
                if (!user.ConfirmationDate.HasValue)
                    throw new BusinessException("Email was not confirmed.");

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

        protected string ApiUrl
        {
            get
            {
                if (_apiUrl == null)
                    _apiUrl = Configuration.GetSection("Url:Api").Get<string>();
                return _apiUrl;
            }
        }

        protected string WebUrl
        {
            get
            {
                if (_webUrl == null)
                    _webUrl = Configuration.GetSection("Url:Web").Get<string>();
                return _webUrl;
            }
        }

        protected string Web3Url
        {
            get
            {
                if (_web3Url == null)
                    _web3Url = Configuration.GetSection("Url:Web3").Get<string>();
                return _web3Url;
            }
        }

        protected string Web3Route
        {
            get
            {
                if (_web3Route == null)
                    _web3Route = Configuration.GetSection("Url:Web3Route").Get<string>();
                return _web3Route;
            }
        }

        protected string StorageConfiguration
        {
            get
            {
                if (_storageConfiguration == null)
                    _storageConfiguration = Configuration.GetSection("ConnectionString:Storage").Get<string>();
                return _storageConfiguration;
            }
        }

        protected string SendGridKey
        {
            get
            {
                if (_sendGridKey == null)
                    _sendGridKey = Configuration.GetSection("Email:SendGridKey").Get<string>();
                return _sendGridKey;
            }
        }

        protected string HashSecret
        {
            get
            {
                if (_hashSecret == null)
                    _hashSecret = Configuration.GetSection("Security:HashSecret").Get<string>();
                return _hashSecret;
            }
        }

        protected int MinimumAucLogin
        {
            get
            {
                if (_minimumAucLogin == null)
                    _minimumAucLogin = Configuration.GetSection("Auth:MinimumAuc").Get<int>();
                return _minimumAucLogin.Value;
            }
        }

        protected int MinimumTimeInSecondsBetweenAdvices
        {
            get
            {
                if (_minimumTimeInSecondsBetweenAdvices == null)
                    _minimumTimeInSecondsBetweenAdvices = Configuration.GetSection("MinimumTimeInSecondsBetweenAdvices").Get<int>();
                return _minimumTimeInSecondsBetweenAdvices.Value;
            }
        }

        protected List<string> EmailErrorList
        {
            get
            {
                if (_emailErrorList == null)
                    _emailErrorList = Configuration.GetSection("Email:Error").Get<List<string>>();
                return _emailErrorList;
            }
        }

        protected TransactionalDapperCommand TransactionalDapperCommand
        {
            get
            {
                return new TransactionalDapperCommand(Configuration);
            }
        }

        protected UserBusiness UserBusiness
        {
            get
            {
                if (_userBusiness == null)
                    _userBusiness = new UserBusiness(Configuration, ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _userBusiness;
            }
        }

        protected PasswordRecoveryBusiness PasswordRecoveryBusiness
        {
            get
            {
                if (_passwordRecoveryBusiness == null)
                    _passwordRecoveryBusiness = new PasswordRecoveryBusiness(Configuration, ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _passwordRecoveryBusiness;
            }
        }

        protected AdvisorBusiness AdvisorBusiness
        {
            get
            {
                if (_advisorBusiness == null)
                    _advisorBusiness = new AdvisorBusiness(Configuration, ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _advisorBusiness;
            }
        }

        protected AdviceBusiness AdviceBusiness
        {
            get
            {
                if (_adviceBusiness == null)
                    _adviceBusiness = new AdviceBusiness(Configuration, ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _adviceBusiness;
            }
        }

        protected FollowBusiness FollowBusiness
        {
            get
            {
                if (_followBusiness == null)
                    _followBusiness = new FollowBusiness(Configuration, ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _followBusiness;
            }
        }

        protected FollowAssetBusiness FollowAssetBusiness
        {
            get
            {
                if (_followAssetBusiness == null)
                    _followAssetBusiness = new FollowAssetBusiness(Configuration, ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _followAssetBusiness;
            }
        }

        protected FollowAdvisorBusiness FollowAdvisorBusiness
        {
            get
            {
                if (_followAdvisorBusiness == null)
                    _followAdvisorBusiness = new FollowAdvisorBusiness(Configuration, ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _followAdvisorBusiness;
            }
        }

        protected AssetBusiness AssetBusiness
        {
            get
            {
                if (_assetBusiness == null)
                    _assetBusiness = new AssetBusiness(Configuration, ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _assetBusiness;
            }
        }

        protected AssetValueBusiness AssetValueBusiness
        {
            get
            {
                if (_assetValueBusiness == null)
                    _assetValueBusiness = new AssetValueBusiness(Configuration, ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _assetValueBusiness;
            }
        }

        protected ExchangeApiAccessBusiness ExchangeApiAccessBusiness
        {
            get
            {
                if (_exchangeApiAccessBusiness == null)
                    _exchangeApiAccessBusiness = new ExchangeApiAccessBusiness(Configuration, ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _exchangeApiAccessBusiness;
            }
        }

        protected RequestToBeAdvisorBusiness RequestToBeAdvisorBusiness
        {
            get
            {
                if (_requestToBeAdvisorBusiness == null)
                    _requestToBeAdvisorBusiness = new RequestToBeAdvisorBusiness(Configuration, ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _requestToBeAdvisorBusiness;
            }
        }

        protected WalletBusiness WalletBusiness
        {
            get
            {
                if (_walletBusiness == null)
                    _walletBusiness = new WalletBusiness(Configuration, ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _walletBusiness;
            }
        }

        protected ActionBusiness ActionBusiness
        {
            get
            {
                if (_actionBusiness == null)
                    _actionBusiness = new ActionBusiness(Configuration, ServiceProvider, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _actionBusiness;
            }
        }

        protected Web3Business Web3Business
        {
            get
            {
                if (_web3Business == null)
                    _web3Business = new Web3Business(Configuration);
                return _web3Business;
            }
        }
    }
}
