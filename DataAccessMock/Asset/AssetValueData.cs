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
            return AllValues().Where(c => c.AssetId == assetId).OrderByDescending(c => c.Date).FirstOrDefault(c => c.Date <= GetDateTimeNow());
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
        
        public List<AssetValue> FilterAssetValues(Dictionary<int, DateTime> assetsMap)
        {
            return AllValues().Where(c => assetsMap.ContainsKey(c.AssetId) && assetsMap[c.AssetId] <= c.Date).ToList();
        }

        private List<AssetValue> AllValues()
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
            return values;
        }
    }
}
