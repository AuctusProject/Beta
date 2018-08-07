using Auctus.DataAccessInterfaces.Asset;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Asset
{
    public class AssetData : IAssetData<DomainObjects.Asset.Asset>
    {
        public void Delete(DomainObjects.Asset.Asset obj)
        {
            throw new NotImplementedException();
        }

        public void Insert(DomainObjects.Asset.Asset obj)
        {
            throw new NotImplementedException();
        }

        public Task InsertManyAsync(IEnumerable<DomainObjects.Asset.Asset> objs)
        {
            throw new NotImplementedException();
        }

        public Task InsertOneAsync(DomainObjects.Asset.Asset obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DomainObjects.Asset.Asset> SelectAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DomainObjects.Asset.Asset> SelectByObject(DomainObjects.Asset.Asset criteria)
        {
            throw new NotImplementedException();
        }

        public void Update(DomainObjects.Asset.Asset obj)
        {
            throw new NotImplementedException();
        }
    }
}
