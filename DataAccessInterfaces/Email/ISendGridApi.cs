using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.DataAccessInterfaces.Email
{
    public interface ISendGridApi
    {
        Task IncludeEmail(string email, string firstName, string lastName);
    }
}
