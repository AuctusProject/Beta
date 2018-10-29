using Auctus.DomainObjects.Advisor;
using Auctus.Model;
using Auctus.Test.Asset;
using Auctus.Util.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Auctus.Test.Advisor
{
    public class AdvisorTest : BaseTest
    {
        [Fact]
        public void CalculationForListAdvisors()
        {
            var response = AdvisorBusiness.ListAdvisorsData();
            Assert.Equal(3, response.Count());
            for (var i = 1; i <= 3; ++i)
            {
                Assert.Single(response.Where(c => c.UserId == i));
                var advisor = response.Single(c => c.UserId == i);
                switch (i)
                {
                    case 1:
                        AssertAdvisor1Data(advisor);
                        break;
                    case 2:
                        AssertAdvisor2Data(advisor);
                        break;
                    case 3:
                        AssertAdvisor3Data(advisor);
                        break;
                }
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void CalculationForAdvisor(int advisorId)
        {
            var response = AdvisorBusiness.GetAdvisorData(advisorId);
            Assert.Equal(advisorId, response.UserId);
            switch (advisorId)
            {
                case 1:
                    AssertAdvisor1Data(response);
                    Assert.Equal(2, response.Assets.Count);
                    Assert.Single(response.Assets.Where(c => c.AssetId == 1));
                    Assert.Single(response.Assets.Single(c => c.AssetId == 1).AssetAdvisor.Where(c => c.UserId == 1));
                    Assert.Single(response.Assets.Where(c => c.AssetId == 2));
                    Assert.Single(response.Assets.Single(c => c.AssetId == 2).AssetAdvisor.Where(c => c.UserId == 1));
                    AssetTest.AssertBaseAsset1Data(response.Assets.Single(c => c.AssetId == 1));
                    AssertAssetAdvisor1Data(response.Assets.Single(c => c.AssetId == 1).AssetAdvisor.Single(c => c.UserId == 1), 1);
                    AssertAssetAdvisor1AdvicesData(response.Assets.Single(c => c.AssetId == 1).AssetAdvisor.Single(c => c.UserId == 1).Advices, 1);
                    AssetTest.AssertBaseAsset2Data(response.Assets.Single(c => c.AssetId == 2));
                    AssertAssetAdvisor1Data(response.Assets.Single(c => c.AssetId == 2).AssetAdvisor.Single(c => c.UserId == 1), 2);
                    AssertAssetAdvisor1AdvicesData(response.Assets.Single(c => c.AssetId == 2).AssetAdvisor.Single(c => c.UserId == 1).Advices, 2);
                    break;
                case 2:
                    AssertAdvisor2Data(response);
                    Assert.Equal(4, response.Assets.Count);
                    Assert.Single(response.Assets.Where(c => c.AssetId == 1));
                    Assert.Single(response.Assets.Where(c => c.AssetId == 2));
                    Assert.Single(response.Assets.Where(c => c.AssetId == 3));
                    Assert.Single(response.Assets.Where(c => c.AssetId == 4));
                    AssetTest.AssertBaseAsset1Data(response.Assets.Single(c => c.AssetId == 1));
                    AssertAssetAdvisor2Data(response.Assets.Single(c => c.AssetId == 1).AssetAdvisor.Single(c => c.UserId == 2), 1);
                    AssertAssetAdvisor2AdvicesData(response.Assets.Single(c => c.AssetId == 1).AssetAdvisor.Single(c => c.UserId == 2).Advices, 1);
                    AssetTest.AssertBaseAsset2Data(response.Assets.Single(c => c.AssetId == 2));
                    AssertAssetAdvisor2Data(response.Assets.Single(c => c.AssetId == 2).AssetAdvisor.Single(c => c.UserId == 2), 2);
                    AssertAssetAdvisor2AdvicesData(response.Assets.Single(c => c.AssetId == 2).AssetAdvisor.Single(c => c.UserId == 2).Advices, 2);
                    AssetTest.AssertBaseAsset3Data(response.Assets.Single(c => c.AssetId == 3));
                    AssertAssetAdvisor2Data(response.Assets.Single(c => c.AssetId == 3).AssetAdvisor.Single(c => c.UserId == 2), 3);
                    AssertAssetAdvisor2AdvicesData(response.Assets.Single(c => c.AssetId == 3).AssetAdvisor.Single(c => c.UserId == 2).Advices, 3);
                    AssetTest.AssertBaseAsset4Data(response.Assets.Single(c => c.AssetId == 4));
                    AssertAssetAdvisor2Data(response.Assets.Single(c => c.AssetId == 4).AssetAdvisor.Single(c => c.UserId == 2), 4);
                    AssertAssetAdvisor2AdvicesData(response.Assets.Single(c => c.AssetId == 4).AssetAdvisor.Single(c => c.UserId == 2).Advices, 4);
                    break;
                case 3:
                    AssertAdvisor3Data(response);
                    Assert.Single(response.Assets);
                    Assert.Single(response.Assets.Where(c => c.AssetId == 1));
                    AssetTest.AssertBaseAsset1Data(response.Assets.Single(c => c.AssetId == 1));
                    AssertAssetAdvisor3Data(response.Assets.Single(c => c.AssetId == 1).AssetAdvisor.Single(c => c.UserId == 3), 1);
                    AssertAssetAdvisor3AdvicesData(response.Assets.Single(c => c.AssetId == 1).AssetAdvisor.Single(c => c.UserId == 3).Advices, 1);
                    break;
            }
        }

        internal static void AssertAdvisor1Data(AdvisorResponse advisor)
        {
            var expRecommendation = new Dictionary<int, double>();
            expRecommendation[AdviceType.Buy.Value] = 7;
            expRecommendation[AdviceType.Sell.Value] = 2;
            AssertAdvisorData(advisor, -0.047146, 0.333333, 4, 2, true, false, 3.743845, 3, expRecommendation);
        }

        internal static void AssertAdvisor2Data(AdvisorResponse advisor)
        {
            var expRecommendation = new Dictionary<int, double>();
            expRecommendation[AdviceType.Buy.Value] = 5;
            expRecommendation[AdviceType.Sell.Value] = 7;
            AssertAdvisorData(advisor, -0.014205, 0.583333, 3, 4, false, false, 3.968860, 2, expRecommendation);
        }

        internal static void AssertAdvisor3Data(AdvisorResponse advisor)
        {
            var expRecommendation = new Dictionary<int, double>();
            expRecommendation[AdviceType.Sell.Value] = 1;
            AssertAdvisorData(advisor, 0.070949, 1, 1, 1, false, true, 4.751651, 1, expRecommendation);
        }

        private static void AssertAdvisorData(AdvisorResponse advisor, double expReturn, double expSuccessRate, int expNumberFollowers,
            int expAssetsAmount, bool expIsOwner, bool expIsFollowing, double rating, int ranking, Dictionary<int, double> expRecommendation)
        {
            Assert.Equal(expReturn, advisor.AverageReturn, 6);
            Assert.Equal(expSuccessRate, advisor.SuccessRate, 6);
            Assert.Equal(expNumberFollowers, advisor.NumberOfFollowers);
            Assert.Equal(expAssetsAmount, advisor.TotalAssetsAdvised);
            Assert.Equal(expIsOwner, advisor.Owner);
            Assert.Equal(expIsFollowing, advisor.Following);
            Assert.Equal(rating, advisor.Rating, 6);
            Assert.Equal(ranking, advisor.Ranking);
            Assert.Equal(expRecommendation.Count, advisor.RecommendationDistribution.Count);
            foreach (var recommendation in expRecommendation)
            {
                Assert.Single(advisor.RecommendationDistribution, c => c.Type == recommendation.Key);
                Assert.Equal(recommendation.Value, advisor.RecommendationDistribution.Single(c => c.Type == recommendation.Key).Total, 2);
            }
        }

        internal static void AssertAssetAdvisor1Data(AssetResponse.AssetAdvisorResponse assetAdvisor, int assetId)
        {
            Assert.Contains(new int[] { 1, 2 }, c => c == assetId);
            switch(assetId)
            {
                case 1:
                    AssertAssetAdvisorData(assetAdvisor, -0.060381, 0.333333, 8, 0, 0, 0.005996);
                    break;
                case 2:
                    AssertAssetAdvisorData(assetAdvisor, -0.020676, 0.333333, 5, 2, 2, 0);
                    break;
            }
        }

        internal static void AssertAssetAdvisor2Data(AssetResponse.AssetAdvisorResponse assetAdvisor, int assetId)
        {
            Assert.Contains(new int[] { 1, 2, 3, 4 }, c => c == assetId);
            switch (assetId)
            {
                case 1:
                    AssertAssetAdvisorData(assetAdvisor, 0.024781, 0.333333, 4, 2, 2, 0);
                    break;
                case 2:
                    AssertAssetAdvisorData(assetAdvisor, 0.062164, 0.666667, 3, 3, 0, 0.167553);
                    break;
                case 3:
                    AssertAssetAdvisorData(assetAdvisor, 0.021150, 0.8, 5, 1, 0, 0.045660);
                    break;
                case 4:
                    AssertAssetAdvisorData(assetAdvisor, -0.537053, 0, 4, 1, 2, 0);
                    break;
            }
        }

        internal static void AssertAssetAdvisor3Data(AssetResponse.AssetAdvisorResponse assetAdvisor, int assetId)
        {
            Assert.Equal(1, assetId);
            AssertAssetAdvisorData(assetAdvisor, 0.070949, 1, 2, 2, 2, 0);
        }

        private static void AssertAssetAdvisorData(AssetResponse.AssetAdvisorResponse assetAdvisor, double expReturn, double expSuccessRate, int expRatings, 
            int expLastMode, int expLastType, double expCurrentReturn)
        {
            Assert.Equal(expReturn, assetAdvisor.AverageReturn, 6);
            Assert.Equal(expSuccessRate, assetAdvisor.SuccessRate, 6);
            Assert.Equal(expCurrentReturn, assetAdvisor.CurrentReturn, 6);
            Assert.Equal(expRatings, assetAdvisor.TotalRatings);
            Assert.Equal(expLastMode, assetAdvisor.LastAdviceMode);
            Assert.Equal(expLastType, assetAdvisor.LastAdviceType);
        }

        internal static void AssertAssetAdvisor1AdvicesData(List<AssetResponse.AdviceResponse> advices, int assetId)
        {
            Assert.Contains(new int[] { 1, 2 }, c => c == assetId);
            switch (assetId)
            {
                case 1:
                    AssertAssetAdvisorAdvicesData(advices, 8, 1, 0);
                    break;
                case 2:
                    AssertAssetAdvisorAdvicesData(advices, 5, 1, 2);
                    break;
            }
        }

        internal static void AssertAssetAdvisor2AdvicesData(List<AssetResponse.AdviceResponse> advices, int assetId)
        {
            Assert.Contains(new int[] { 1, 2, 3, 4 }, c => c == assetId);
            switch (assetId)
            {
                case 1:
                    AssertAssetAdvisorAdvicesData(advices, 4, 1, 2);
                    break;
                case 2:
                    AssertAssetAdvisorAdvicesData(advices, 3, 1, 0);
                    break;
                case 3:
                    AssertAssetAdvisorAdvicesData(advices, 5, 0, 0);
                    break;
                case 4:
                    AssertAssetAdvisorAdvicesData(advices, 4, 2, 2);
                    break;
            }
        }

        internal static void AssertAssetAdvisor3AdvicesData(List<AssetResponse.AdviceResponse> advices, int assetId)
        {
            Assert.Equal(1, assetId);
            AssertAssetAdvisorAdvicesData(advices, 2, 0, 2);
        }

        private static void AssertAssetAdvisorAdvicesData(List<AssetResponse.AdviceResponse> advices, int expCount, int expFirstType, int expLastType)
        {
            Assert.Equal(expCount, advices.Count);
            Assert.Equal(expFirstType, advices.First().AdviceType);
            Assert.Equal(expLastType, advices.Last().AdviceType);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void Advise(int assetId)
        {
            switch(assetId)
            {
                case 1:
                    AdvisorBusiness.Advise(assetId, AdviceType.ClosePosition, null, null);
                    AdvisorBusiness.Advise(assetId, AdviceType.Sell, null, null);
                    break;
                case 2:
                    Assert.Throws<BusinessException>(() => AdvisorBusiness.Advise(assetId, AdviceType.Buy, null, null));
                    break;
                case 3:
                    Assert.Throws<BusinessException>(() => AdvisorBusiness.Advise(assetId, AdviceType.ClosePosition, null, null));
                    AdvisorBusiness.Advise(assetId, AdviceType.Buy, null, null);
                    Assert.Throws<BusinessException>(() => AdvisorBusiness.Advise(assetId, AdviceType.Sell, null, null));
                    break;
                case 4:
                    Assert.Throws<BusinessException>(() => AdvisorBusiness.Advise(assetId, AdviceType.ClosePosition, null, null));
                    AdvisorBusiness.Advise(assetId, AdviceType.Buy, null, null);
                    break;
                case 5:
                    Assert.Throws<NotFoundException>(() => AdvisorBusiness.Advise(assetId, AdviceType.Buy, null, null));
                    break;
                default: break;
            }
        }
    }
}
