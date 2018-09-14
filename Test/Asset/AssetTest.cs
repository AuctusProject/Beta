using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Auctus.DomainObjects.Advisor;
using Auctus.Model;
using Auctus.Test.Advisor;

namespace Auctus.Test.Asset
{
    public class AssetTest : BaseTest
    {
        [Fact]
        public void CalculationForListAssets()
        {
            var response = AssetBusiness.ListAssetData();
            Assert.Equal(4, response.Count());
            for (var i = 1; i <= 4; ++i)
            {
                Assert.Single(response.Where(c => c.AssetId == i));
                var asset = response.Single(c => c.AssetId == i);
                switch (i)
                {
                    case 1:
                        AssertBaseAsset1Data(asset);
                        AssertExtraAsset1Data(asset);
                        break;
                    case 2:
                        AssertBaseAsset2Data(asset);
                        AssertExtraAsset2Data(asset);
                        break;
                    case 3:
                        AssertBaseAsset3Data(asset);
                        AssertExtraAsset3Data(asset);
                        break;
                    case 4:
                        AssertBaseAsset4Data(asset);
                        AssertExtraAsset4Data(asset);
                        break;
                }
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void CalculationForAsset(int assetId)
        {
            var response = AssetBusiness.GetAssetData(assetId);
            Assert.Equal(assetId, response.AssetId);
            switch (assetId)
            {
                case 1:
                    AssertBaseAsset1Data(response);
                    AssertExtraAsset1Data(response);
                    AssertAsset1AdvicesData(response.Advices);
                    Assert.Equal(3, response.Advisors.Count);
                    Assert.Equal(3, response.AssetAdvisor.Count);
                    Assert.Single(response.Advisors.Where(c => c.UserId == 1));
                    Assert.Single(response.Advisors.Where(c => c.UserId == 2));
                    Assert.Single(response.Advisors.Where(c => c.UserId == 3));
                    Assert.Single(response.AssetAdvisor.Where(c => c.UserId == 1));
                    Assert.Single(response.AssetAdvisor.Where(c => c.UserId == 2));
                    Assert.Single(response.AssetAdvisor.Where(c => c.UserId == 3));
                    AdvisorTest.AssertAdvisor1Data(response.Advisors.Single(c => c.UserId == 1));
                    AdvisorTest.AssertAdvisor2Data(response.Advisors.Single(c => c.UserId == 2));
                    AdvisorTest.AssertAdvisor3Data(response.Advisors.Single(c => c.UserId == 3));
                    AdvisorTest.AssertAssetAdvisor1Data(response.AssetAdvisor.Single(c => c.UserId == 1), 1);
                    AdvisorTest.AssertAssetAdvisor2Data(response.AssetAdvisor.Single(c => c.UserId == 2), 1);
                    AdvisorTest.AssertAssetAdvisor3Data(response.AssetAdvisor.Single(c => c.UserId == 3), 1);
                    break;
                case 2:
                    AssertBaseAsset2Data(response);
                    AssertExtraAsset2Data(response);
                    AssertAsset2AdvicesData(response.Advices);
                    Assert.Equal(2, response.Advisors.Count);
                    Assert.Equal(2, response.AssetAdvisor.Count);
                    Assert.Single(response.Advisors.Where(c => c.UserId == 1));
                    Assert.Single(response.Advisors.Where(c => c.UserId == 2));
                    Assert.Single(response.AssetAdvisor.Where(c => c.UserId == 1));
                    Assert.Single(response.AssetAdvisor.Where(c => c.UserId == 2));
                    AdvisorTest.AssertAdvisor1Data(response.Advisors.Single(c => c.UserId == 1));
                    AdvisorTest.AssertAdvisor2Data(response.Advisors.Single(c => c.UserId == 2));
                    AdvisorTest.AssertAssetAdvisor1Data(response.AssetAdvisor.Single(c => c.UserId == 1), 2);
                    AdvisorTest.AssertAssetAdvisor2Data(response.AssetAdvisor.Single(c => c.UserId == 2), 2);
                    break;
                case 3:
                    AssertBaseAsset3Data(response);
                    AssertExtraAsset3Data(response);
                    AssertAsset3AdvicesData(response.Advices);
                    Assert.Single(response.Advisors);
                    Assert.Single(response.AssetAdvisor);
                    Assert.Single(response.Advisors.Where(c => c.UserId == 2));
                    Assert.Single(response.AssetAdvisor.Where(c => c.UserId == 2));
                    AdvisorTest.AssertAdvisor2Data(response.Advisors.Single(c => c.UserId == 2));
                    AdvisorTest.AssertAssetAdvisor2Data(response.AssetAdvisor.Single(c => c.UserId == 2), 3);
                    break;
                case 4:
                    AssertBaseAsset4Data(response);
                    AssertExtraAsset4Data(response);
                    AssertAsset4AdvicesData(response.Advices);
                    Assert.Single(response.Advisors);
                    Assert.Single(response.AssetAdvisor);
                    Assert.Single(response.Advisors.Where(c => c.UserId == 2));
                    Assert.Single(response.AssetAdvisor.Where(c => c.UserId == 2));
                    AdvisorTest.AssertAdvisor2Data(response.Advisors.Single(c => c.UserId == 2));
                    AdvisorTest.AssertAssetAdvisor2Data(response.AssetAdvisor.Single(c => c.UserId == 2), 4);
                    break;
            }
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

        internal static void AssertExtraAsset1Data(AssetResponse asset)
        {
            var expRecommendation = new Dictionary<int, double>();
            expRecommendation[AdviceType.Sell.Value] = 1;
            expRecommendation[AdviceType.ClosePosition.Value] = 2;
            AssertExtraAssetData(asset, 5, true, 5, expRecommendation);
        }

        internal static void AssertExtraAsset2Data(AssetResponse asset)
        {
            var expRecommendation = new Dictionary<int, double>();
            expRecommendation[AdviceType.Sell.Value] = 1;
            expRecommendation[AdviceType.ClosePosition.Value] = 1;
            AssertExtraAssetData(asset, 1, true, 0, expRecommendation);
        }

        internal static void AssertExtraAsset3Data(AssetResponse asset)
        {
            var expRecommendation = new Dictionary<int, double>();
            expRecommendation[AdviceType.Sell.Value] = 1;
            AssertExtraAssetData(asset, 0, false, 3, expRecommendation);
        }

        internal static void AssertExtraAsset4Data(AssetResponse asset)
        {
            var expRecommendation = new Dictionary<int, double>();
            expRecommendation[AdviceType.ClosePosition.Value] = 1;
            AssertExtraAssetData(asset, 0, false, 0, expRecommendation);
        }

        internal static void AssertAsset1AdvicesData(List<AssetResponse.AdviceResponse> advices)
        {
            var expAdvisorCount = new Dictionary<int, int>();
            expAdvisorCount[1] = 8;
            expAdvisorCount[2] = 4;
            expAdvisorCount[3] = 2;
            AssertAssetAdvicesData(advices, 14, 1, 0, expAdvisorCount);
        }

        internal static void AssertAsset2AdvicesData(List<AssetResponse.AdviceResponse> advices)
        {
            var expAdvisorCount = new Dictionary<int, int>();
            expAdvisorCount[1] = 5;
            expAdvisorCount[2] = 3;
            AssertAssetAdvicesData(advices, 8, 1, 2, expAdvisorCount);
        }

        internal static void AssertAsset3AdvicesData(List<AssetResponse.AdviceResponse> advices)
        {
            var expAdvisorCount = new Dictionary<int, int>();
            expAdvisorCount[2] = 5;
            AssertAssetAdvicesData(advices, 5, 0, 0, expAdvisorCount);
        }

        internal static void AssertAsset4AdvicesData(List<AssetResponse.AdviceResponse> advices)
        {
            var expAdvisorCount = new Dictionary<int, int>();
            expAdvisorCount[2] = 2;
            AssertAssetAdvicesData(advices, 2, 1, 2, expAdvisorCount);
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

        private static void AssertExtraAssetData(AssetResponse asset, int expFollowers, bool expIsFollowing, int expMode, Dictionary<int, double> expRecommendation)
        {
            Assert.Equal(expFollowers, asset.NumberOfFollowers);
            Assert.Equal(expIsFollowing, asset.Following);
            Assert.Equal(expMode, asset.Mode);
            Assert.Equal(expRecommendation.Count, asset.RecommendationDistribution.Count);
            foreach (var recommendation in expRecommendation)
            {
                Assert.Single(asset.RecommendationDistribution, c => c.Type == recommendation.Key);
                Assert.Equal(recommendation.Value, asset.RecommendationDistribution.Single(c => c.Type == recommendation.Key).Total, 2);
            }
        }

        private static void AssertAssetAdvicesData(List<AssetResponse.AdviceResponse> advices, int expCount, int expFirstType, int expLastType, Dictionary<int, int> expAdvisorCount)
        {
            Assert.Equal(expCount, advices.Count);
            Assert.Equal(expFirstType, advices.First().AdviceType);
            Assert.Equal(expLastType, advices.Last().AdviceType);
            var advisorData = advices.GroupBy(c => c.UserId).Select(c => new { UserId = c.Key, Total = c.Count() });
            Assert.Equal(expAdvisorCount.Count, advisorData.Count());
            foreach (var advisorCount in expAdvisorCount)
            {
                Assert.Single(advisorData, c => c.UserId == advisorCount.Key);
                Assert.Equal(advisorCount.Value, advisorData.Single(c => c.UserId == advisorCount.Key).Total);
            }
        }
    }
}
