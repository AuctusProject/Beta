using Auctus.DataAccess.Core;
using Auctus.DomainObjects.Follow;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Follow
{
    public class FollowAssetData : BaseSQL<FollowAsset>
    {
        public override string TableName => "FollowAsset";
    }
}
