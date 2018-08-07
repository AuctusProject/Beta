using Auctus.DomainObjects.Account;
using DataAccessInterfaces;

namespace Auctus.DataAccessInterfaces.Account
{
    public interface IUserData<T> : IBaseData<T>
    {
        User GetForLogin(string email);
        User GetByConfirmationCode(string code);
        User GetForNewWallet(string email);
        User GetByEmail(string email);
    }
}