using Auctus.DataAccessInterfaces.Blockchain;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Auctus.Business.Blockchain
{
    public class Web3Business
    {
        private readonly IWeb3Api Api;

        internal Web3Business(IConfigurationRoot configuration, IServiceProvider serviceProvider)
        {
            Api = (IWeb3Api)serviceProvider.GetService(typeof(IWeb3Api));
        }

        public decimal GetAucAmount(string address)
        {
            return Api.GetAucAmount(address);
        }
    }
}
