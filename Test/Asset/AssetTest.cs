using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Auctus.Test.Asset
{
    public class AssetTest : BaseTest
    {
        [Fact]
        public void CalculationForListAssets()
        {
            var response = AssetBusiness.ListAssetData();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void CalculationForAsset(int assetId)
        {
            var response = AssetBusiness.GetAssetData(assetId);
        }
    }
}
