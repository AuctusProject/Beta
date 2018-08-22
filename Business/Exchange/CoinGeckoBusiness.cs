using Auctus.DataAccessInterfaces.Exchange;
using Auctus.DomainObjects.Exchange;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Exchange
{
    public class CoinGeckoBusiness
    {
        private readonly ICoinGeckoApi Api;

        internal CoinGeckoBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider)
        {
            Api = (ICoinGeckoApi)serviceProvider.GetService(typeof(ICoinGeckoApi));
        }

        public IEnumerable<AssetResult> GetAllCoinsData()
        {
            return Api.GetAllCoinsData();
        }
    }
}
