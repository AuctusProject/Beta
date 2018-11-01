using Auctus.Business.Account;
using Auctus.Business.Advisor;
using Auctus.Business.Asset;
using Auctus.Business.Blockchain;
using Auctus.Business.Email;
using Auctus.Business.Event;
using Auctus.Business.Exchange;
using Auctus.Business.News;
using Auctus.Business.Storage;
using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces;
using Auctus.DomainObjects.Account;
using Auctus.Util;
using Auctus.Util.Exceptions;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.Business
{
    public abstract class BaseBusiness<T, D> where D : IBaseData<T>
    {
        protected IConfigurationRoot Configuration { get; private set; }
        protected IServiceProvider ServiceProvider { get; private set; }
        protected IServiceScopeFactory ServiceScopeFactory { get; private set; }
        protected ILoggerFactory LoggerFactory { get; private set; }
        protected ILogger Logger { get; private set; }
        protected Cache MemoryCache { get; private set; }
        protected string LoggedEmail { get; private set; }
        protected string LoggedIp { get; private set; }

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
        private EmailBusiness _emailBusiness;
        private CoinMarketcapBusiness _coinMarketcapBusiness;
        private CoinGeckoBusiness _coinGeckoBusiness;
        private AzureStorageBusiness _azureStorageBusiness;
        private FacebookBusiness _facebookBusiness;
        private GoogleBusiness _googleBusiness;
        private AssetCurrentValueBusiness _assetCurrentValueBusiness;
        private AgencyBusiness _agencyBusiness;
        private AgencyRatingBusiness _agencyRatingBusiness;
        private ReportBusiness _reportBusiness;
        private CoinMarketCalBusiness _coinMarketCalBusiness;
        private AssetEventBusiness _assetEventBusiness;
        private AssetEventCategoryBusiness _assetEventCategoryBusiness;
        private NewsBusiness _newsBusiness;
        private NewsSourceBusiness _newsSourceBusiness;
        private NewsRssBusiness _newsRssBusiness;
        private ExchangeBusiness _exchangeBusiness;
        private PairBusiness _pairBusiness;
        private BinanceBusiness _binanceBusiness;

        private string _apiUrl;
        private string _webUrl;
        private int? _minimumAucLogin;
        private int? _minimumDaysToKeepAuc;
        private int? _minimumTimeInSecondsBetweenAdvices;
        private string _hashSecret;
        private double? _discountPercentageOnAuc;
        private int? _assetUSDId;
        private int? _assetBTCId;
        private int? _assetETHId;
        private List<string> _admins;
        private List<TerminalAssetConfig> _terminalAssets;

        protected class TerminalAssetConfig
        {
            public string Id { get; set; }
            public string ChartPair { get; set; }
            public string ChartExchange { get; set; }

            public int AssetId { get { return !string.IsNullOrEmpty(Id) ? int.Parse(Id) : 0;  } }
        }

        protected BaseBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip)
        {
            Configuration = configuration;
            ServiceProvider = serviceProvider;
            ServiceScopeFactory = serviceScopeFactory;
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

        public User GetLoggedUser()
        {
            if (string.IsNullOrEmpty(LoggedEmail))
                return null;

            var cacheKey = GetUserCacheKey(LoggedEmail);
            var user = MemoryCache.Get<User>(cacheKey);
            if (user == null)
            {
                UserBusiness.EmailValidation(LoggedEmail);
                user = UserBusiness.GetByEmail(LoggedEmail);
            }
            else if (UserBusiness.IsValidAdvisor(user))
                user = MemoryCache.Get<DomainObjects.Advisor.Advisor>(cacheKey);
            return user;
        }

        public User GetValidUser()
        {
            var cacheKey = GetUserCacheKey(LoggedEmail);
            var user = MemoryCache.Get<User>(cacheKey);
            if (user == null)
            {
                UserBusiness.EmailValidation(LoggedEmail);
                user = UserBusiness.GetByEmail(LoggedEmail);
                if (user == null)
                    throw new NotFoundException("User cannot be found.");

                if (!UserBusiness.IsValidAdvisor(user))
                    MemoryCache.Set<User>(cacheKey, user);
                else
                    MemoryCache.Set<DomainObjects.Advisor.Advisor>(cacheKey, (DomainObjects.Advisor.Advisor)user);
                return user;
            }
            else
            {
                if (UserBusiness.IsValidAdvisor(user))
                    user = MemoryCache.Get<DomainObjects.Advisor.Advisor>(cacheKey);
                return user;
            }
        }

        protected string GetUserCacheKey(string userEmail)
        {
            UserBusiness.BaseEmailValidation(userEmail);
            return userEmail.ToLower().Trim();
        }

        protected void RunAsync(System.Action action)
        {
            Task.Factory.StartNew(() =>
            {
                using (var scope = ServiceScopeFactory.CreateScope())
                {
                    ServiceProvider = scope.ServiceProvider;
                    TelemetryClient telemetry = new TelemetryClient();
                    try
                    {
                        telemetry.TrackEvent(action.Method.Name);
                        Logger.LogInformation($"Job {action.Method.Name} started.");
                        action();
                        Logger.LogInformation($"Job {action.Method.Name} ended.");
                    }
                    catch (Exception e)
                    {
                        telemetry.TrackException(e);
                        Logger.LogCritical(e, $"Exception on {action.Method.Name} job");
                    }
                    finally
                    {
                        telemetry.Flush();
                    }
                }
            });
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
                    _webUrl = Configuration.GetSection("Url:Web").Get<List<string>>().First();
                return _webUrl;
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

        protected int MinimumDaysToKeepAuc
        {
            get
            {
                if (_minimumDaysToKeepAuc == null)
                    _minimumDaysToKeepAuc = Configuration.GetSection("MinimumDaysToKeepAuc").Get<int>();
                return _minimumDaysToKeepAuc.Value;
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

        protected double DiscountPercentageOnAuc
        {
            get
            {
                if (_discountPercentageOnAuc == null)
                    _discountPercentageOnAuc = Configuration.GetSection("DiscountPercentageOnAuc").Get<double>();
                return _discountPercentageOnAuc.Value;
            }
        }

        protected int AssetUSDId
        {
            get
            {
                if (_assetUSDId == null)
                    _assetUSDId = Configuration.GetSection("AssetUSDId").Get<int>();
                return _assetUSDId.Value;
            }
        }

        protected int AssetBTCId
        {
            get
            {
                if (_assetBTCId == null)
                    _assetBTCId = Configuration.GetSection("AssetBTCId").Get<int>();
                return _assetBTCId.Value;
            }
        }

        protected int AssetETHId
        {
            get
            {
                if (_assetETHId == null)
                    _assetETHId = Configuration.GetSection("AssetETHId").Get<int>();
                return _assetETHId.Value;
            }
        }

        protected List<string> Admins
        {
            get
            {
                if (_admins == null)
                    _admins = Configuration.GetSection("Admins").Get<List<string>>();
                return _admins;
            }
        }

        protected List<TerminalAssetConfig> TerminalAssets
        {
            get
            {
                if (_terminalAssets == null)
                    _terminalAssets = Configuration.GetSection("TerminalAssets").Get<List<TerminalAssetConfig>>();
                return _terminalAssets;
            }
        }

        protected bool IsAdmin
        {
            get
            {
                return !string.IsNullOrWhiteSpace(LoggedEmail) && Admins?.Any(c => c == LoggedEmail) == true;
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

        protected AdviceBusiness AdviceBusiness
        {
            get
            {
                if (_adviceBusiness == null)
                    _adviceBusiness = new AdviceBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _adviceBusiness;
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

        protected RequestToBeAdvisorBusiness RequestToBeAdvisorBusiness
        {
            get
            {
                if (_requestToBeAdvisorBusiness == null)
                    _requestToBeAdvisorBusiness = new RequestToBeAdvisorBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _requestToBeAdvisorBusiness;
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

        protected Web3Business Web3Business
        {
            get
            {
                if (_web3Business == null)
                    _web3Business = new Web3Business(Configuration, ServiceProvider);
                return _web3Business;
            }
        }

        protected EmailBusiness EmailBusiness
        {
            get
            {
                if (_emailBusiness == null)
                    _emailBusiness = new EmailBusiness(Configuration, ServiceProvider);
                return _emailBusiness;
            }
        }

        protected CoinMarketcapBusiness CoinMarketcapBusiness
        {
            get
            {
                if (_coinMarketcapBusiness == null)
                    _coinMarketcapBusiness = new CoinMarketcapBusiness(Configuration, ServiceProvider);
                return _coinMarketcapBusiness;
            }
        }

        protected CoinGeckoBusiness CoinGeckoBusiness
        {
            get
            {
                if (_coinGeckoBusiness == null)
                    _coinGeckoBusiness = new CoinGeckoBusiness(Configuration, ServiceProvider);
                return _coinGeckoBusiness;
            }
        }

        protected AzureStorageBusiness AzureStorageBusiness
        {
            get
            {
                if (_azureStorageBusiness == null)
                    _azureStorageBusiness = new AzureStorageBusiness(Configuration, ServiceProvider);
                return _azureStorageBusiness;
            }
        }

        protected FacebookBusiness FacebookBusiness
        {
            get
            {
                if (_facebookBusiness == null)
                    _facebookBusiness = new FacebookBusiness(Configuration, ServiceProvider);
                return _facebookBusiness;
            }
        }

        protected GoogleBusiness GoogleBusiness
        {
            get
            {
                if (_googleBusiness == null)
                    _googleBusiness = new GoogleBusiness(Configuration, ServiceProvider);
                return _googleBusiness;
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

        protected CoinMarketCalBusiness CoinMarketCalBusiness
        {
            get
            {
                if (_coinMarketCalBusiness == null)
                    _coinMarketCalBusiness = new CoinMarketCalBusiness(Configuration, ServiceProvider);
                return _coinMarketCalBusiness;
            }
        }

        protected AssetEventCategoryBusiness AssetEventCategoryBusiness
        {
            get
            {
                if (_assetEventCategoryBusiness == null)
                    _assetEventCategoryBusiness = new AssetEventCategoryBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _assetEventCategoryBusiness;
            }
        }

        protected AssetEventBusiness AssetEventBusiness
        {
            get
            {
                if (_assetEventBusiness == null)
                    _assetEventBusiness = new AssetEventBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _assetEventBusiness;
            }
        }

        protected NewsBusiness NewsBusiness
        {
            get
            {
                if (_newsBusiness == null)
                    _newsBusiness = new NewsBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _newsBusiness;
            }
        }

        protected NewsSourceBusiness NewsSourceBusiness
        {
            get
            {
                if (_newsSourceBusiness == null)
                    _newsSourceBusiness = new NewsSourceBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _newsSourceBusiness;
            }
        }

        protected NewsRssBusiness NewsRssBusiness
        {
            get
            {
                if (_newsRssBusiness == null)
                    _newsRssBusiness = new NewsRssBusiness(Configuration, ServiceProvider);
                return _newsRssBusiness;
            }
        }

        protected ExchangeBusiness ExchangeBusiness
        {
            get
            {
                if (_exchangeBusiness == null)
                    _exchangeBusiness = new ExchangeBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _exchangeBusiness;
            }
        }

        protected PairBusiness PairBusiness
        {
            get
            {
                if (_pairBusiness == null)
                    _pairBusiness = new PairBusiness(Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, LoggedEmail, LoggedIp);
                return _pairBusiness;
            }
        }

        protected BinanceBusiness BinanceBusiness
        {
            get
            {
                if (_binanceBusiness == null)
                    _binanceBusiness = new BinanceBusiness(Configuration, ServiceProvider);
                return _binanceBusiness;
            }
        }
    }
}