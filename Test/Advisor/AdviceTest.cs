using Auctus.DomainObjects.Advisor;
using Auctus.Model;
using Auctus.Test.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Auctus.Test.Advisor
{
    public class AdviceTest : BaseTest
    {
        //[Theory]
        //[InlineData(null, null)]
        //[InlineData(10, null)]
        //[InlineData(3, 11)]
        //public void ListLastAdvicesForUserWithPagination(int? top, int? lastId)
        //{
        //    var response = AdviceBusiness.ListLastAdvicesForUserWithPagination(top, lastId);
        //    if (top.HasValue)
        //        Assert.Equal(response.Count(), top);
        //    if (lastId.HasValue)
        //        Assert.True(response.All(c => c.Id < lastId));

        //    for (var i = 1; i < response.Count(); i++)
        //    {
        //        Assert.True(response.ElementAt(i).CreationDate <= response.ElementAt(i - 1).CreationDate);
        //    }
        //}

        //[Theory]
        //[InlineData(null, null)]
        //[InlineData(10, null)]
        //[InlineData(3, 11)]

        //public void ListFeed(int? top, int? lastId)
        //{
        //    var response = AdviceBusiness.ListFeed(top, lastId);
        //    if (top.HasValue)
        //        Assert.Equal(response.Count(), top);
        //    if (lastId.HasValue)
        //        Assert.True(response.All(c => c.AdviceId < lastId));

        //    for (var i = 1; i<response.Count(); i++)
        //    {
        //        Assert.True(response.ElementAt(i).AdviceDate <= response.ElementAt(i - 1).AdviceDate);
        //    }

        //    response.ToList().ForEach(f => AssertPropertiesAreFilled(f));

        //    var newLastId = response.Last().AdviceId;
        //    var newResponse = AdviceBusiness.ListFeed(top, newLastId);

        //    var responseIds = response.Select(c => c.AdviceId);
        //    var newResponseIds = newResponse.Select(c => c.AdviceId);
        //    Assert.All(newResponseIds, (id) => Assert.DoesNotContain(id, responseIds));
        //}

        //private void AssertPropertiesAreFilled(FeedResponse feedResponse)
        //{
        //    Assert.NotEqual(0, feedResponse.AdvisorRanking);
        //    Assert.NotNull(feedResponse.AdvisorName);
        //    Assert.NotNull(feedResponse.AssetName);
        //    Assert.NotNull(feedResponse.AssetCode);
        //    Assert.True(feedResponse.FollowingAdvisor || feedResponse.FollowingAsset);
        //}
    }
}
