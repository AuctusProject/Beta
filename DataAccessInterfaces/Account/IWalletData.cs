using Auctus.DomainObjects.Account;

namespace Auctus.DataAccessInterfaces.Account
{
    public interface IWalletData<T> : IBaseData<T>
    {
        Wallet GetByUser(int userId);
        Wallet GetByAddress(string address);
    }
}