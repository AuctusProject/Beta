using Auctus.DataAccessInterfaces.Exchange;
using Auctus.DomainObjects.Exchange;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Exchange
{
    public class CoinMarketcapBusiness
    {
        private readonly ICoinMarketcapApi Api;

        internal CoinMarketcapBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider)
        {
            Api = (ICoinMarketcapApi)serviceProvider.GetService(typeof(ICoinMarketcapApi));
        }

        public IEnumerable<AssetResult> GetAllCoinsData()
        {
            return Api.GetAllCoinsData();
        }
    }
}
