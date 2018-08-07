using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Asset;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Asset
{
    public class AssetValueData : IAssetValueData<AssetValue>
    {
        public void Delete(AssetValue obj)
        {
            throw new NotImplementedException();
        }

        public AssetValue GetLastValue(int assetId)
        {
            throw new NotImplementedException();
        }

        public void Insert(AssetValue obj)
        {
            throw new NotImplementedException();
        }

        public Task InsertManyAsync(IEnumerable<AssetValue> objs)
        {
            throw new NotImplementedException();
        }

        public Task InsertOneAsync(AssetValue obj)
        {
            throw new NotImplementedException();
        }

        public List<AssetValue> List(IEnumerable<int> assetsIds, DateTime? startDate = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AssetValue> SelectAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AssetValue> SelectByObject(AssetValue criteria)
        {
            throw new NotImplementedException();
        }

        public void Update(AssetValue obj)
        {
            throw new NotImplementedException();
        }
    }
}
