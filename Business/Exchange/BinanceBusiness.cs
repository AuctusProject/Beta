using Auctus.DataAccess.Exchange;
using Auctus.DataAccessInterfaces.Exchange;
using Auctus.DomainObjects.Exchange;
using Auctus.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Auctus.Business.Exchange
{
    public class BinanceBusiness
    {
        private readonly IBinanceApi Api;

        public BinanceBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider)
        {
            Api = (IBinanceApi)serviceProvider.GetService(typeof(IBinanceApi));
        }

        public BinanceApi.ExchangeInfo GetExchangeInfo()
        {
            return null;// Api.GetExchangeInfo();
        }
    }
}
