using Auctus.DataAccess.Account;
using Auctus.DataAccess.Advisor;
using Auctus.DataAccess.Asset;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
using Auctus.DomainObjects.Asset;
using DataAccessInterfaces.Account;
using DataAccessInterfaces.Advisor;
using DataAccessInterfaces.Asset;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auctus.Business
{
    public static class DataAccessDependencyResolver
    {
        public static void RegisterDataAccess(IServiceCollection services, IConfigurationRoot configuration)
        {
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
        }
    }
}
