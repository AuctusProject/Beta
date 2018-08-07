using Auctus.DomainObjects.Account;
using DataAccessInterfaces;

namespace Auctus.DataAccessInterfaces.Account
{
    public interface IWalletData<T> : IBaseData<T>
    {
        Wallet GetByUser(int userId);
        Wallet GetByAddress(string address);
    }
}