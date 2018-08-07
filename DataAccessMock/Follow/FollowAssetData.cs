using Auctus.DataAccessInterfaces.Follow;
using Auctus.DomainObjects.Follow;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Follow
{
    public class FollowAssetData : IFollowAssetData<FollowAsset>
    {
        public void Delete(FollowAsset obj)
        {
            throw new NotImplementedException();
        }

        public void Insert(FollowAsset obj)
        {
            throw new NotImplementedException();
        }

        public Task InsertManyAsync(IEnumerable<FollowAsset> objs)
        {
            throw new NotImplementedException();
        }

        public Task InsertOneAsync(FollowAsset obj)
        {
            throw new NotImplementedException();
        }

        public List<FollowAsset> List(IEnumerable<int> assetsIds)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FollowAsset> SelectAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FollowAsset> SelectByObject(FollowAsset criteria)
        {
            throw new NotImplementedException();
        }

        public void Update(FollowAsset obj)
        {
            throw new NotImplementedException();
        }
    }
}
