using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Exchange;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessInterfaces.Account
{
    public interface IGoogleApi
    {
        SocialUser GetSocialUser(String accessToken);
    }
}
