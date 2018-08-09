using Auctus.DataAccess.Core;
using Auctus.DomainObjects.Account;
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

        public FollowAsset Create(int userId, int AssetId, FollowActionType actionType)
        {
            using (var transaction = new TransactionalDapperCommand())
            {
                var follow = FollowBusiness.Create(userId, actionType);
                transaction.Insert(follow);
                FollowAsset followAsset = new FollowAsset()
                {
                    Id = follow.Id,
                    AssetId = AssetId,
                    ActionType = follow.ActionType,
                    UserId = follow.UserId,
                    CreationDate = follow.CreationDate
                };
                transaction.Insert(followAsset);
                transaction.Commit();

                return followAsset;
            }
        }
    }
}
