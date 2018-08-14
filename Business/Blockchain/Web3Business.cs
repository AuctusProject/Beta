using Auctus.DataAccess.Blockchain;
using Auctus.DomainObjects.Web3;
using Auctus.Util;
using Auctus.Util.NotShared;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
        private readonly Web3Api Data;

        internal Web3Business(IConfigurationRoot configuration)
        {
            Data = new Web3Api(configuration);
        }

        public decimal GetAucAmount(string address)
        {
            return Data.GetAucAmount(address);
        }
    }
}
