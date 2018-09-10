using Auctus.DomainObjects.Account;
using System.Collections.Generic;

namespace Auctus.DataAccessInterfaces.Account
{
    public interface IUserData<T> : IBaseData<T>
    {
        User GetById(int id);
        User GetForLogin(string email);
        User GetByConfirmationCode(string code);
        User GetForNewWallet(string email);
        User GetByEmail(string email);
        List<User> ListForAucSituation();
        User GetByReferralCode(string referralCode);
        List<User> ListReferredUsers(int referredId);
        List<User> ListUsersFollowingAdvisorOrAsset(int advisorId, int assetId);
        List<User> ListAllUsersData();
        User GetForLoginById(int id);
    }
}