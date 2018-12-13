using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auctus.DataAccessMock.Asset
{
    public class AssetCurrentValueData : BaseData<DomainObjects.Asset.AssetCurrentValue>, IAssetCurrentValueData<DomainObjects.Asset.AssetCurrentValue>
    {
        public List<AssetCurrentValue> AssetCurrentValues
        {
            get
            {
                var result = new List<AssetCurrentValue>();
                foreach (var asset in AssetData.AllAssets)
                {
                    result.Add(new AssetCurrentValue()
                    {
                        Id = asset.Id,
                        Name = asset.Name,
                        Code = asset.Code,
                        Type = asset.Type,
                        ShortSellingEnabled = asset.ShortSellingEnabled,
                        CoinGeckoId = asset.CoinGeckoId,
                        MarketCap = asset.MarketCap
                    });
                }
                foreach (var assetValue in result)
                {
                    if (assetValue.Id == 1)
                    {
                        assetValue.UpdateDate = GetDateTimeNow();
                        assetValue.CurrentValue = 7461.011245767;
                        assetValue.Variation24Hours = 0.00085;
                        assetValue.Variation7Days = -0.012279;
                        assetValue.Variation30Days = -0.124007;
                    }
                    else if (assetValue.Id == 2)
                    {
                        assetValue.UpdateDate = GetDateTimeNow();
                        assetValue.CurrentValue = 595.28951907;
                        assetValue.Variation24Hours = 0.015921;
                        assetValue.Variation7Days = 0.023787;
                        assetValue.Variation30Days = -0.168164;
                    }
                    else if (assetValue.Id == 3)
                    {
                        assetValue.UpdateDate = GetDateTimeNow();
                        assetValue.CurrentValue = 0.667077627;
                        assetValue.Variation24Hours = 0.034764;
                        assetValue.Variation7Days = 0.070678;
                        assetValue.Variation30Days = -0.047862;
                    }
                    else if (assetValue.Id == 4)
                    {
                        assetValue.UpdateDate = GetDateTimeNow();
                        assetValue.CurrentValue = 0.25015462;
                        assetValue.Variation24Hours = -0.025706;
                        assetValue.Variation7Days = -0.00787;
                        assetValue.Variation30Days = -0.55687;
                    }
                }
                return result;
            }
        }

        public List<AssetCurrentValue> ListAllAssets(bool enabled, IEnumerable<int> ids)
        {
            return ids == null || !ids.Any() ? AssetCurrentValues : AssetCurrentValues.Where(c => ids.Contains(c.Id)).ToList();
        }

        public List<AssetCurrentValue> ListAssetsFollowedByUser(int userId)
        {
            return AssetCurrentValues;
        }

        public void UpdateAssetValue(IEnumerable<AssetCurrentValue> assetCurrentValues)
        {
            throw new NotImplementedException();
        }

        public void UpdateAssetValue7And30Days(IEnumerable<AssetCurrentValue> assetCurrentValues)
        {
            throw new NotImplementedException();
        }

        public void UpdateFullAssetValue(IEnumerable<AssetCurrentValue> assetCurrentValues)
        {
            throw new NotImplementedException();
        }
    }
}
