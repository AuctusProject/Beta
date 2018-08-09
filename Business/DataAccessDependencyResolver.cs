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
using Microsoft.Extensions.DependencyInjection;

namespace Auctus.Business
{
    public static class DataAccessDependencyResolver
    {
        public static void RegisterDataAccess(IServiceCollection services)
        {
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
            services.AddScoped<IFollowData<Follow>, FollowData>();
        }
    }
}
