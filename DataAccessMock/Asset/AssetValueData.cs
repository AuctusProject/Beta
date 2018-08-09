using Auctus.DataAccessInterfaces.Asset;
using Auctus.DataAccessMock.Asset.Data;
using Auctus.DomainObjects.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Asset
{
    public class AssetValueData : BaseData<AssetValue>, IAssetValueData<AssetValue>
    {
        public AssetValue GetLastValue(int assetId)
        {
            throw new NotImplementedException();
        }

        internal static AssetValue GetAssetValue(int assetId, double value, DateTime dateTime)
        {
            return new AssetValue()
            {
                AssetId = assetId,
                Date = dateTime,
                Value = value
            };
        }
        
        public List<AssetValue> List(IEnumerable<int> assetsIds, DateTime? startDate = null)
        {
            var values = new List<AssetValue>();
            values.AddRange(AssetValuesPartialData.GetAssetValues1());
            values.AddRange(AssetValuesPartialData.GetAssetValues2());
            values.AddRange(AssetValuesPartialData.GetAssetValues3());
            values.AddRange(AssetValuesPartialData.GetAssetValues4());
            values.AddRange(AssetValuesPartialData.GetAssetValues5());
            values.AddRange(AssetValuesPartialData.GetAssetValues6());
            values.AddRange(AssetValuesPartialData.GetAssetValues7());
            values.AddRange(AssetValuesPartialData.GetAssetValues8());
            values.AddRange(AssetValuesPartialData.GetAssetValues9());
            values.AddRange(AssetValuesPartialData.GetAssetValues10());
            values.AddRange(AssetValuesPartialData.GetAssetValues11());
            values.AddRange(AssetValuesPartialData.GetAssetValues12());
            return values.Where(c => assetsIds.Contains(c.AssetId) && (!startDate.HasValue || c.Date >= startDate.Value)).ToList();
        }
    }
}
