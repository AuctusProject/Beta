using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auctus.Util;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    public class AssetV1Controller : AssetBaseController
    {
        protected AssetV1Controller(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider) : base(loggerFactory, cache, serviceProvider)
        {
        }
    }
}
