using Auctus.DomainObjects.Asset;
using Auctus.Util;
using DataAccessInterfaces.Asset;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Asset
{
    public class FollowAssetBusiness : BaseBusiness<FollowAsset, IFollowAssetData<FollowAsset>>
    {
        public FollowAssetBusiness(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(serviceProvider, loggerFactory, cache, email, ip) { }

        public List<FollowAsset> ListFollowers(IEnumerable<int> assetIds = null)
        {
            return Data.ListFollowers(assetIds);
        }
    }
}
