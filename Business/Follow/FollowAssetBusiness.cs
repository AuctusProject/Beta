using Auctus.DataAccess.Follow;
using Auctus.DataAccessInterfaces.Follow;
using Auctus.DomainObjects.Follow;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Follow
{
    public class FollowAssetBusiness : BaseBusiness<FollowAsset, IFollowAssetData<FollowAsset>>
    {
        public FollowAssetBusiness(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(serviceProvider, loggerFactory, cache, email, ip) { }

        public List<FollowAsset> List(IEnumerable<int> assetIds = null)
        {
            return Data.List(assetIds);
        }
    }
}
