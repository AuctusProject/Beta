using Auctus.Business.Account;
using Auctus.Business.Advisor;
using Auctus.Business.Asset;
using Auctus.Business.Follow;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DataAccessInterfaces.Asset;
using Auctus.DataAccessInterfaces.Follow;
using Auctus.DataAccessMock.Account;
using Auctus.DataAccessMock.Advisor;
using Auctus.DataAccessMock.Asset;
using Auctus.DataAccessMock.Follow;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
using Auctus.DomainObjects.Asset;
using Auctus.DomainObjects.Follow;
using Auctus.Util;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Auctus.Test
{
    public abstract class BaseTest
    {
        private readonly IServiceProvider ServiceProvider;
        private readonly ILoggerFactory LoggerFactory;
        private readonly Cache MemoryCache;
        private readonly string LoggedEmail;
        private readonly string LoggedIp;

        public BaseTest()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddMemoryCache();
            services.AddSingleton<Cache>();

            services.AddScoped<IActionData<DomainObjects.Account.Action>, ActionData>();
            services.AddScoped<IExchangeApiAccessData<ExchangeApiAccess>, ExchangeApiAccessData>();
            services.AddScoped<IPasswordRecoveryData<PasswordRecovery>, PasswordRecoveryData>();
            services.AddScoped<IUserData<User>, UserData>();
            services.AddScoped<IWalletData<Wallet>, WalletData>();
            services.AddScoped<IAdviceData<Advice>, AdviceData>();
            services.AddScoped<IAdvisorData<DomainObjects.Advisor.Advisor>, AdvisorData>();
            services.AddScoped<IRequestToBeAdvisorData<RequestToBeAdvisor>, RequestToBeAdvisorData>();
            services.AddScoped<IAssetData<DomainObjects.Asset.Asset>, AssetData>();
            services.AddScoped<IAssetValueData<AssetValue>, AssetValueData>();
            services.AddScoped<IFollowAdvisorData<FollowAdvisor>, FollowAdvisorData>();
            services.AddScoped<IFollowAssetData<FollowAsset>, FollowAssetData>();
            services.AddScoped<IFollowData<DomainObjects.Follow.Follow>, FollowData>();

            ServiceProvider = services.BuildServiceProvider();
            MemoryCache = ServiceProvider.GetService<Cache>();
            LoggerFactory = new LoggerFactory();
            LoggedEmail = "thiagomvaruajo@gmail.com";
            LoggedIp = "10.0.0.1";
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
