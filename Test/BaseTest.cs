using Auctus.Business.Account;
using Auctus.Business.Advisor;
using Auctus.Business.Asset;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DataAccessInterfaces.Asset;
using Auctus.DataAccessInterfaces.Blockchain;
using Auctus.DataAccessInterfaces.Email;
using Auctus.DataAccessInterfaces.Event;
using Auctus.DataAccessInterfaces.Exchange;
using Auctus.DataAccessInterfaces.Storage;
using Auctus.DataAccessInterfaces.News;
using Auctus.DataAccessInterfaces.Trade;
using Auctus.DataAccessMock.Account;
using Auctus.DataAccessMock.Advisor;
using Auctus.DataAccessMock.Asset;
using Auctus.DataAccessMock.Blockchain;
using Auctus.DataAccessMock.Email;
using Auctus.DataAccessMock.Exchange;
using Auctus.DataAccessMock.Storage;
using Auctus.DataAccessMock.Event;
using Auctus.DataAccessMock.Trade;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
using Auctus.DomainObjects.Asset;
using Auctus.DomainObjects.Exchange;
using Auctus.DomainObjects.Event;
using Auctus.DomainObjects.Trade;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using Auctus.Business.Trade;
using Auctus.DataAccessInterfaces;
using Auctus.DataAccessMock;

namespace Auctus.Test
{
    public abstract class BaseTest : IDisposable
    {
        private readonly IServiceProvider ServiceProvider;
        private readonly IServiceScopeFactory ServiceScopeFactory;
        private readonly ILoggerFactory LoggerFactory;
        private readonly IConfigurationRoot Configuration;
        private readonly Cache MemoryCache;
        protected string LoggedEmail;
        private readonly string LoggedIp;

        protected BaseTest()
        {
            var rootPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..\\..\\..\\..\\Api");
            var builder = new ConfigurationBuilder()
                .SetBasePath(rootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            IServiceCollection services = new ServiceCollection();
            services.AddMemoryCache();
            services.AddSingleton<Cache>();

            services.AddSingleton<IEmailResource, EmailResource>();
            services.AddSingleton<IWeb3Api, Web3Api>();
            services.AddSingleton<IAzureStorageResource, AzureStorageResource>();
            services.AddSingleton<ICoinMarketcapApi, CoinMarketcapApi>();
            services.AddSingleton<ICoinGeckoApi, CoinGeckoApi>();
            services.AddSingleton<IBinanceApi, BinanceApi>();
            services.AddSingleton<ICoinMarketCalApi, CoinMarketCalApi>();
            services.AddScoped<ITransactionalDapperCommand>(c => new TransactionalDapperCommand(services.BuildServiceProvider()));
            services.AddScoped<IActionData<DomainObjects.Account.Action>, ActionData>();
            services.AddScoped<IExchangeApiAccessData<ExchangeApiAccess>, ExchangeApiAccessData>();
            services.AddScoped<IPasswordRecoveryData<PasswordRecovery>, PasswordRecoveryData>();
            services.AddScoped<IUserData<User>, UserData>();
            services.AddScoped<IWalletData<Wallet>, WalletData>();
            services.AddScoped<IAdvisorData<DomainObjects.Advisor.Advisor>, AdvisorData>();
            services.AddScoped<IAssetData<DomainObjects.Asset.Asset>, AssetData>();
            services.AddScoped<IAssetValueData<AssetValue>, AssetValueData>();
            services.AddScoped<IFollowAdvisorData<FollowAdvisor>, FollowAdvisorData>();
            services.AddScoped<IFollowAssetData<FollowAsset>, FollowAssetData>();
            services.AddScoped<IFollowData<Follow>, FollowData>();
            services.AddScoped<IAssetCurrentValueData<AssetCurrentValue>, AssetCurrentValueData>();
            services.AddScoped<IAgencyData<Agency>, AgencyData>();
            services.AddScoped<IAgencyRatingData<AgencyRating>, AgencyRatingData>();
            services.AddScoped<IReportData<Report>, ReportData>();
            services.AddScoped<IAssetEventCategoryData<AssetEventCategory>, AssetEventCategoryData>();
            services.AddScoped<IAssetEventData<AssetEvent>, AssetEventData>();
            services.AddScoped<ILinkEventAssetData<LinkEventAsset>, LinkEventAssetData>();
            services.AddScoped<ILinkEventCategoryData<LinkEventCategory>, LinkEventCategoryData>();
            services.AddScoped<INewsData<DomainObjects.News.News>, NewsData>(c => new NewsData());
            services.AddScoped<INewsSourceData<DomainObjects.News.NewsSource>, NewsSourceData>(c => new NewsSourceData());
            services.AddScoped<INewsRss, NewsRss>(c => new NewsRss());
            services.AddScoped<IExchangeData<DomainObjects.Exchange.Exchange>, ExchangeData>();
            services.AddScoped<IPairData<Pair>, PairData>();
            services.AddScoped<IOrderData<Order>, OrderData>();
            services.AddScoped<IAdvisorRankingData<AdvisorRanking>, AdvisorRankingData>();
            services.AddScoped<IAdvisorProfitData<AdvisorProfit>, AdvisorProfitData>();
            services.AddScoped<IAdvisorRankingHistoryData<AdvisorRankingHistory>, AdvisorRankingHistoryData>();
            services.AddScoped<IAdvisorProfitHistoryData<AdvisorProfitHistory>, AdvisorProfitHistoryData>();
            services.AddScoped<IAdvisorMonthlyRankingData<AdvisorMonthlyRanking>, AdvisorMonthlyRankingData>();

            ServiceProvider = services.BuildServiceProvider();
            ServiceScopeFactory = new ServiceScopeFactory(ServiceProvider);
            MemoryCache = ServiceProvider.GetRequiredService<Cache>();
            LoggerFactory = new LoggerFactory();
            LoggedEmail = "test@auctus.org";
            LoggedIp = "10.0.0.1";
        }

        public virtual void Dispose()
        {
        }

        private UserBusiness _userBusiness;
        private PasswordRecoveryBusiness _passwordRecoveryBusiness;
        private AdvisorBusiness _advisorBusiness;
        private AdvisorProfitBusiness _advisorProfitBusiness;
        private AssetBusiness _assetBusiness;
        private AssetValueBusiness _assetValueBusiness;
        private FollowBusiness _followBusiness;
        private FollowAssetBusiness _followAssetBusiness;
        private FollowAdvisorBusiness _followAdvisorBusiness;
        private ExchangeApiAccessBusiness _exchangeApiAccessBusiness;
        private WalletBusiness _walletBusiness;
        private ActionBusiness _actionBusiness;
        private AssetCurrentValueBusiness _assetCurrentValueBusiness;
        private AgencyBusiness _agencyBusiness;
        private AgencyRatingBusiness _agencyRatingBusiness;
        private ReportBusiness _reportBusiness;
        private OrderBusiness _orderBusiness;

        protected UserBusiness UserBusiness
        {
            get
            {
                if (_userBusiness == null)
                    _userBusiness = new UserBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _userBusiness;
            }
        }

        protected PasswordRecoveryBusiness PasswordRecoveryBusiness
        {
            get
            {
                if (_passwordRecoveryBusiness == null)
                    _passwordRecoveryBusiness = new PasswordRecoveryBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _passwordRecoveryBusiness;
            }
        }

        protected AdvisorBusiness AdvisorBusiness
        {
            get
            {
                if (_advisorBusiness == null)
                    _advisorBusiness = new AdvisorBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _advisorBusiness;
            }
        }

        protected AdvisorProfitBusiness AdvisorProfitBusiness
        {
            get
            {
                if (_advisorProfitBusiness == null)
                    _advisorProfitBusiness = new AdvisorProfitBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _advisorProfitBusiness;
            }
        }

        protected FollowBusiness FollowBusiness
        {
            get
            {
                if (_followBusiness == null)
                    _followBusiness = new FollowBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _followBusiness;
            }
        }

        protected FollowAssetBusiness FollowAssetBusiness
        {
            get
            {
                if (_followAssetBusiness == null)
                    _followAssetBusiness = new FollowAssetBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _followAssetBusiness;
            }
        }

        protected FollowAdvisorBusiness FollowAdvisorBusiness
        {
            get
            {
                if (_followAdvisorBusiness == null)
                    _followAdvisorBusiness = new FollowAdvisorBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _followAdvisorBusiness;
            }
        }

        protected AssetBusiness AssetBusiness
        {
            get
            {
                if (_assetBusiness == null)
                    _assetBusiness = new AssetBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _assetBusiness;
            }
        }

        protected AssetValueBusiness AssetValueBusiness
        {
            get
            {
                if (_assetValueBusiness == null)
                    _assetValueBusiness = new AssetValueBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _assetValueBusiness;
            }
        }

        protected ExchangeApiAccessBusiness ExchangeApiAccessBusiness
        {
            get
            {
                if (_exchangeApiAccessBusiness == null)
                    _exchangeApiAccessBusiness = new ExchangeApiAccessBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _exchangeApiAccessBusiness;
            }
        }

        protected WalletBusiness WalletBusiness
        {
            get
            {
                if (_walletBusiness == null)
                    _walletBusiness = new WalletBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _walletBusiness;
            }
        }

        protected ActionBusiness ActionBusiness
        {
            get
            {
                if (_actionBusiness == null)
                    _actionBusiness = new ActionBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _actionBusiness;
            }
        }

        protected AssetCurrentValueBusiness AssetCurrentValueBusiness
        {
            get
            {
                if (_assetCurrentValueBusiness == null)
                    _assetCurrentValueBusiness = new AssetCurrentValueBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _assetCurrentValueBusiness;
            }
        }

        protected AgencyBusiness AgencyBusiness
        {
            get
            {
                if (_agencyBusiness == null)
                    _agencyBusiness = new AgencyBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _agencyBusiness;
            }
        }

        protected AgencyRatingBusiness AgencyRatingBusiness
        {
            get
            {
                if (_agencyRatingBusiness == null)
                    _agencyRatingBusiness = new AgencyRatingBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _agencyRatingBusiness;
            }
        }

        protected ReportBusiness ReportBusiness
        {
            get
            {
                if (_reportBusiness == null)
                    _reportBusiness = new ReportBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _reportBusiness;
            }
        }

        protected OrderBusiness OrderBusiness
        {
            get
            {
                if (_orderBusiness == null)
                    _orderBusiness = new OrderBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _orderBusiness;
            }
        }
    }
}
