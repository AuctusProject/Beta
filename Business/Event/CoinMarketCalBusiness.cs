using Auctus.DataAccessInterfaces.Event;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Event
{
    public class CoinMarketCalBusiness
    {
        private readonly ICoinMarketCalApi Api;

        internal CoinMarketCalBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider)
        {
            Api = (ICoinMarketCalApi)serviceProvider.GetService(typeof(ICoinMarketCalApi));
        }
    }
}
