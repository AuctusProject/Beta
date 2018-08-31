using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
using Auctus.DomainObjects.Asset;
using Auctus.Model;
using Auctus.Util;
using Auctus.Util.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Auctus.Business.Account
{
    public class GoogleBusiness
    {
        private readonly IGoogleApi Api;

        internal GoogleBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider)
        {
            Api = (IGoogleApi)serviceProvider.GetService(typeof(IGoogleApi));
        }

        internal SocialUser GetSocialUser(string accessToken)
        {
            return Api.GetSocialUser(accessToken);
        }
    }
}
