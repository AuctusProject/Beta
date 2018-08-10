using System;
using System.Collections.Generic;
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
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void CalculationForAdvisor(int advisorId)
        {
            var response = AdvisorBusiness.GetAdvisorData(advisorId);
        }
    }
}
