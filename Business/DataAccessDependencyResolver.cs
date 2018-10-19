using Auctus.DataAccess.Account;
using Auctus.DataAccess.Advisor;
using Auctus.DataAccess.Asset;
using Auctus.DataAccess.Blockchain;
using Auctus.DataAccess.Email;
using Auctus.DataAccess.Event;
using Auctus.DataAccess.Exchange;
using Auctus.DataAccess.News;
using Auctus.DataAccess.Storage;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DataAccessInterfaces.Asset;
using Auctus.DataAccessInterfaces.Blockchain;
using Auctus.DataAccessInterfaces.Email;
using Auctus.DataAccessInterfaces.Event;
using Auctus.DataAccessInterfaces.Exchange;
using Auctus.DataAccessInterfaces.News;
using Auctus.DataAccessInterfaces.Storage;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
using Auctus.DomainObjects.Asset;
using Auctus.DomainObjects.Event;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auctus.Business
{
    public static class DataAccessDependencyResolver
    {
        public static void RegisterDataAccess(IServiceCollection services, IConfigurationRoot configuration, bool isDevelopment)
        {
            services.AddSingleton<IEmailResource, EmailResource>(c => new EmailResource(configuration, isDevelopment));
            services.AddSingleton<IWeb3Api, Web3Api>(c => new Web3Api(configuration));
            services.AddSingleton<IAzureStorageResource, AzureStorageResource>(c => new AzureStorageResource(configuration));
            services.AddSingleton<ICoinMarketcapApi, CoinMarketCapApi>(c => new CoinMarketCapApi());
            services.AddSingleton<ICoinGeckoApi, CoinGeckoApi>(c => new CoinGeckoApi());
            services.AddSingleton<IGoogleApi, GoogleApi>(c => new GoogleApi(configuration));
            services.AddSingleton<IFacebookApi, FacebookApi>(c => new FacebookApi(configuration));
            services.AddSingleton<ICoinMarketCalApi, CoinMarketCalApi>(c => new CoinMarketCalApi(configuration, services.BuildServiceProvider().GetRequiredService<Cache>()));
            services.AddScoped<IActionData<DomainObjects.Account.Action>, ActionData>(c => new ActionData(configuration));
            services.AddScoped<IExchangeApiAccessData<ExchangeApiAccess>, ExchangeApiAccessData>(c => new ExchangeApiAccessData(configuration));
            services.AddScoped<IPasswordRecoveryData<PasswordRecovery>, PasswordRecoveryData>(c => new PasswordRecoveryData(configuration));
            services.AddScoped<IUserData<User>, UserData>(c => new UserData(configuration));
            services.AddScoped<IWalletData<Wallet>, WalletData>(c => new WalletData(configuration));
            services.AddScoped<IAdviceData<Advice>, AdviceData>(c => new AdviceData(configuration));
            services.AddScoped<IAdvisorData<DomainObjects.Advisor.Advisor>, AdvisorData>(c => new AdvisorData(configuration));
            services.AddScoped<IRequestToBeAdvisorData<RequestToBeAdvisor>, RequestToBeAdvisorData>(c => new RequestToBeAdvisorData(configuration));
            services.AddScoped<IAssetData<DomainObjects.Asset.Asset>, AssetData>(c => new AssetData(configuration));
            services.AddScoped<IAssetValueData<AssetValue>, AssetValueData>(c => new AssetValueData(configuration));
            services.AddScoped<IFollowAdvisorData<FollowAdvisor>, FollowAdvisorData>(c => new FollowAdvisorData(configuration));
            services.AddScoped<IFollowAssetData<FollowAsset>, FollowAssetData>(c => new FollowAssetData(configuration));
            services.AddScoped<IFollowData<Follow>, FollowData>(c => new FollowData(configuration));
            services.AddScoped<IAssetCurrentValueData<AssetCurrentValue>, AssetCurrentValueData>(c => new AssetCurrentValueData(configuration));
            services.AddScoped<IEarlyAccessEmailData<EarlyAccessEmail>, EarlyAccessEmailData>(c => new EarlyAccessEmailData(configuration));
            services.AddScoped<IAgencyData<Agency>, AgencyData>(c => new AgencyData(configuration));
            services.AddScoped<IAgencyRatingData<AgencyRating>, AgencyRatingData>(c => new AgencyRatingData(configuration));
            services.AddScoped<IReportData<Report>, ReportData>(c => new ReportData(configuration));
            services.AddScoped<IAssetEventCategoryData<AssetEventCategory>, AssetEventCategoryData>(c => new AssetEventCategoryData(configuration));
            services.AddScoped<IAssetEventData<AssetEvent>, AssetEventData>(c => new AssetEventData(configuration));
            services.AddScoped<ILinkEventAssetData<LinkEventAsset>, LinkEventAssetData>(c => new LinkEventAssetData(configuration));
            services.AddScoped<ILinkEventCategoryData<LinkEventCategory>, LinkEventCategoryData>(c => new LinkEventCategoryData(configuration));
            services.AddScoped<INewsData<DomainObjects.News.News>, NewsData>(c => new NewsData(configuration));
            services.AddScoped<INewsSourceData<DomainObjects.News.NewsSource>, NewsSourceData>(c => new NewsSourceData(configuration));
            services.AddScoped<INewsRss, NewsRss>(c => new NewsRss());
        }
    }
}
