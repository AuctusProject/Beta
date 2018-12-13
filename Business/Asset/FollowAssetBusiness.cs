using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Asset;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auctus.Business.Asset
{
    public class FollowAssetBusiness : BaseBusiness<FollowAsset, IFollowAssetData<FollowAsset>>
    {
        public FollowAssetBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public List<FollowAsset> ListFollowers(IEnumerable<int> assetIds = null)
        {
            return Data.ListFollowers(assetIds);
        }

        public List<int> ListAssetsFollowed(int userId)
        {
            return Data.ListAssetsFollowed(userId).Select(c => c.AssetId).ToList();
        }

        public int GetTotalFollowers(int assetId)
        {
            return Data.CountAssetFollowers(assetId);
        }

        public FollowAsset Create(int userId, int AssetId, FollowActionType actionType)
        {
            using (var transaction = TransactionalDapperCommand)
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
