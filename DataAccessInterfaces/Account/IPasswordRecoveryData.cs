namespace Auctus.DataAccessInterfaces.Account
{
    public interface IPasswordRecoveryData<T> : IBaseData<T>
    {
        T Get(int userId);
        T Get(string token);
    }
}