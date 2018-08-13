using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Auctus.DomainObjects.Advisor;
using Auctus.Model;

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

        internal static void AssertValues1Data(AssetResponse asset)
        {
            AssertValuesData(asset, 1020, 9683.955819, 7470.347514);
        }

        internal static void AssertValues2Data(AssetResponse asset)
        {
            AssertValuesData(asset, 1179, 781.064044, 595.091365);
        }

        internal static void AssertValues3Data(AssetResponse asset)
        {
            AssertValuesData(asset, 1149, 0.899942, 0.668940);
        }

        internal static void AssertValues4Data(AssetResponse asset)
        {
            AssertValuesData(asset, 1383, 0.826652, 0.248711);
        }

        internal static void AssertBaseAsset1Data(AssetResponse asset)
        {
            AssertBaseAssetData(asset, 14, 3, 7461.011246, 0.000850, -0.012279, -0.124007);
        }

        internal static void AssertBaseAsset2Data(AssetResponse asset)
        {
            AssertBaseAssetData(asset, 8, 2, 595.289519, 0.015921, 0.023787, -0.168164);
        }

        internal static void AssertBaseAsset3Data(AssetResponse asset)
        {
            AssertBaseAssetData(asset, 5, 1, 0.667078, 0.034764, 0.070678, -0.047862);
        }

        internal static void AssertBaseAsset4Data(AssetResponse asset)
        {
            AssertBaseAssetData(asset, 2, 1, 0.250155, -0.025706, -0.007870, -0.556870);
        }

        private static void AssertBaseAssetData(AssetResponse asset, int expTotalRatings, int expTotalAdvisors, double expLastValue, 
            double expVariation24h, double expVariation7d, double expVariation30d)
        {
            Assert.Equal(expTotalRatings, asset.TotalRatings);
            Assert.Equal(expTotalAdvisors, asset.TotalAdvisors);
            Assert.Equal(expLastValue, asset.LastValue, 6);
            Assert.Equal(expVariation24h, asset.Variation24h.Value, 6);
            Assert.Equal(expVariation7d, asset.Variation7d.Value, 6);
            Assert.Equal(expVariation30d, asset.Variation30d.Value, 6);
        }

        private static void AssertValuesData(AssetResponse asset, int expCount, double expFirstValue, double expLastValue)
        {
            Assert.Equal(expCount, asset.Values.Count);
            Assert.Equal(expFirstValue, asset.Values.First().Value, 6);
            Assert.Equal(expLastValue, asset.Values.Last().Value, 6);
        }
    }
}
