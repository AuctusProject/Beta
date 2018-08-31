using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class DashboardResponse
    {
        public int TotalUsersStartedRegistration { get; set; }
        public int TotalUsersConfirmed { get; set; }
        public int TotalUsersStartedRegistrationFromReferral { get; set; }
        public int TotalUsersConfirmedFromReferral { get; set; }
        public int TotalAdvisors { get; set; }
        public int TotalRequestToBeAdvisor { get; set; }
        public int TotalActiveUsers { get; set; }
        public int TotalActiveAdvisors { get; set; }
        public int TotalWalletsWithAuc { get; set; }
        public int TotalWalletsInProgress { get; set; }
        public double AucRatioPerConfirmedUser { get; set; }
        public double AucRatioPerUserInProgress { get; set; }
        public double AucHolded { get; set; }
        public double AucHoldedInProgress { get; set; }
        public int TotalAssetsAdviced { get; set; }
        public int TotalRecentAdvices { get; set; }
        public int TotalUsersFollowing { get; set; }
        public int TotalAdvisorsFollowed { get; set; }
        public List<DistributionData> ReferralStatus { get; set; } = new List<DistributionData>();
        public int TotalAdvices { get; set; }
        public List<DistributionData> Advices { get; set; } = new List<DistributionData>();
        public int TotalFollowing { get; set; }
        public List<DistributionData> Following { get; set; } = new List<DistributionData>();
        public List<AdvisorData> AdvisorReferral { get; set; } = new List<AdvisorData>();
        public List<AdvisorData> AdvisorAdvices { get; set; } = new List<AdvisorData>();
        public List<AdvisorData> AdvisorFollowers { get; set; } = new List<AdvisorData>();
        public List<RegistrationData> UsersStartedRegistration { get; set; } = new List<RegistrationData>();
        public List<RegistrationData> UsersConfirmed { get; set; } = new List<RegistrationData>();
        public List<RegistrationData> Advisors { get; set; } = new List<RegistrationData>();
        public List<RegistrationData> RequestToBeAdvisor { get; set; } = new List<RegistrationData>();
        public FlagData UsersStartedRegistrationLastSitutation { get; set; }
        public FlagData UsersConfirmedLastSitutation { get; set; }
        public FlagData AdvisorsLastSitutation { get; set; }
        public FlagData RequestToBeAdvisorLastSitutation { get; set; }

        public class DistributionData
        {
            public string Name { get; set; }
            public int Amount { get; set; }
        }
        public class FlagData
        {
            public string Description { get; set; }
            public DateTime Date { get; set; }
        }
        public class RegistrationData
        {
            public DateTime Date { get; set; }
            public int Value { get; set; }
        }
        public class AdvisorData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string UrlGuid { get; set; }
            public int Total { get; set; }
            public int SubValue1 { get; set; }
            public int SubValue2 { get; set; }
            public int SubValue3 { get; set; }
        }
    }
}
