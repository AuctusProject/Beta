using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Exchange;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessInterfaces.Account
{
    public interface IFacebookApi
    {
        SocialUser GetSocialUser(String accessToken);
    }
}
