using Auctus.DomainObjects.Account;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Asset
{
    public class FollowAsset : Follow
    {
        [DapperType(System.Data.DbType.Int32)]
        public int AssetId { get; set; }
    }
}
