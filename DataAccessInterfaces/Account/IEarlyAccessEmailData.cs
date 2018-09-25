using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessInterfaces.Account
{
    public interface IEarlyAccessEmailData<T> : IBaseData<T>
    {
        T GetByEmail(string email);
    }
}
