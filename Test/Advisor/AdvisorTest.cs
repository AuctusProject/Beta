using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Auctus.Test.Advisor
{
    public class AdvisorTest : BaseTest
    {
        [Fact]
        public void Calculation()
        {
            var response = AdvisorBusiness.ListAdvisorsData();
        }
    }
}
