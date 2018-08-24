using Auctus.DataAccess.Account;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
using Auctus.Model;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.Business.Account
{
    public class ActionBusiness : BaseBusiness<DomainObjects.Account.Action, IActionData<DomainObjects.Account.Action>>
    {
        public ActionBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public void InsertNewWallet(DateTime dateTime, int userId, string message, decimal? aucAmount)
        {
            Data.InsertOneAsync(new DomainObjects.Account.Action()
            {
                CreationDate = dateTime,
                UserId = userId,
                Type = ActionType.NewWallet.Value,
                Message = message,
                AucAmount = aucAmount,
                Ip = LoggedIp
            });
        }

        public void InsertNewLogin(int userId, decimal? aucAmount)
        {
            Data.InsertOneAsync(new DomainObjects.Account.Action()
            {
                CreationDate = Data.GetDateTimeNow(),
                UserId = userId,
                Type = ActionType.NewLogin.Value,
                AucAmount = aucAmount,
                Ip = LoggedIp
            });
        }

        public void InsertNewAucVerification(int userId, decimal aucAmount)
        {
            Data.InsertOneAsync(new DomainObjects.Account.Action()
            {
                CreationDate = Data.GetDateTimeNow(),
                UserId = userId,
                Type = ActionType.NewAucVerification.Value,
                AucAmount = aucAmount,
                Ip = LoggedIp
            });
        }

        public void InsertJobAucVerification(int userId, decimal aucAmount)
        {
            Data.InsertOneAsync(new DomainObjects.Account.Action()
            {
                CreationDate = Data.GetDateTimeNow(),
                UserId = userId,
                Type = ActionType.JobVerification.Value,
                AucAmount = aucAmount,
                Ip = LoggedIp
            });
        }

        public DashboardResponse GetDashboardData()
        {
            var cutDayForActivity = Data.GetDateTimeNow().AddDays(-7);
            var assets = AssetBusiness.ListAssets();
            var advisors = AdvisorBusiness.GetAdvisors();
            var users = Task.Factory.StartNew(() => UserBusiness.ListAllUsersData());
            var requestsToBeAdvisor = Task.Factory.StartNew(() => RequestToBeAdvisorBusiness.ListAll());
            var advices = Task.Factory.StartNew(() => AdviceBusiness.List(advisors.Select(c => c.Id).Distinct()));
            var advisorFollowers = Task.Factory.StartNew(() => FollowAdvisorBusiness.ListFollowers(advisors.Select(c => c.Id).Distinct()));
            var assetFollowers = Task.Factory.StartNew(() => FollowAssetBusiness.ListFollowers());
            var activities = Task.Factory.StartNew(() => Data.FilterActivity(cutDayForActivity, ActionType.NewAucVerification, ActionType.NewLogin, ActionType.NewWallet));
            Task.WaitAll(users, requestsToBeAdvisor, advices, advisorFollowers, assetFollowers, activities);

            var result = new DashboardResponse();
            result.TotalUsersConfirmed = users.Result.Count(c => c.Wallets.Any() && !advisors.Any(a => a.Id == c.Id));
            result.TotalUsersConfirmedFromReferral = users.Result.Count(c => c.ReferredId.HasValue && c.Wallets.Any() && !advisors.Any(a => a.Id == c.Id));
            result.TotalUsersStartedRegistration = users.Result.Count(c => !advisors.Any(a => a.Id == c.Id)) - result.TotalUsersConfirmed;
            result.TotalUsersStartedRegistrationFromReferral = users.Result.Count(c => c.ReferredId.HasValue && !advisors.Any(a => a.Id == c.Id)) - result.TotalUsersConfirmedFromReferral;
            result.TotalAdvisors = advisors.Count;
            result.TotalRequestToBeAdvisor = requestsToBeAdvisor.Result.Any() ? requestsToBeAdvisor.Result.Select(c => c.UserId).Distinct().Count() : 0;
            result.TotalActiveUsers = activities.Result.Select(c => c.UserId).Distinct().Count(c => !advisors.Any(a => a.Id == c));
            result.TotalActiveAdvisors = activities.Result.Select(c => c.UserId).Distinct().Count(c => advisors.Any(a => a.Id == c));
            result.TotalWalletsInProgress = users.Result.Count(c => c.ReferralStatusType == ReferralStatusType.InProgress);
            result.TotalAdvices = advices.Result.Count;
            result.TotalAssetsAdviced = advices.Result.Select(c => c.AssetId).Distinct().Count();
            result.TotalRecentAdvices = advices.Result.Count(c => c.CreationDate >= cutDayForActivity);
            result.TotalFollowing = advisorFollowers.Result.Count + assetFollowers.Result.Count;
            result.TotalAdvisorsFollowed = advisorFollowers.Result.Any() ? advisorFollowers.Result.Select(c => c.AdvisorId).Distinct().Count() : 0;
            result.TotalUsersFollowing = advisorFollowers.Result.Select(c => c.UserId).Concat(assetFollowers.Result.Select(c => c.UserId)).Distinct().Count();

            result.Advisors = advisors.GroupBy(c => c.BecameAdvisorDate.ToString("yyyy-MM-dd"))
                .Select(g => new DashboardResponse.RegistrationData() { Date = DateTime.Parse(g.Key), Value = g.Count() }).OrderBy(c => c.Date).ToList();
            result.AdvisorsLastSitutation = GetFlagData(result.Advisors);

            result.UsersStartedRegistration = users.Result.Where(c => !advisors.Any(a => a.Id == c.Id) && !c.Wallets.Any()).GroupBy(c => c.CreationDate.ToString("yyyy-MM-dd"))
                .Select(g => new DashboardResponse.RegistrationData() { Date = DateTime.Parse(g.Key), Value = g.Count() }).OrderBy(c => c.Date).ToList();
            result.UsersStartedRegistrationLastSitutation = GetFlagData(result.UsersStartedRegistration);

            result.RequestToBeAdvisor = requestsToBeAdvisor.Result.GroupBy(c => c.UserId).Select(g => new { Id = g.Key, Date = g.Min(c => c.CreationDate) }).GroupBy(c => c.Date.ToString("yyyy-MM-dd"))
                .Select(g => new DashboardResponse.RegistrationData() { Date = DateTime.Parse(g.Key), Value = g.Count() }).OrderBy(c => c.Date).ToList();
            result.RequestToBeAdvisorLastSitutation = GetFlagData(result.RequestToBeAdvisor);

            var confirmedUsers = new Dictionary<int, DateTime>();
            foreach (var user in users.Result.Where(c => c.Wallets.Any()))
            {
                if (!advisors.Any(c => c.Id == user.Id))
                        confirmedUsers[user.Id] = user.Wallets.OrderBy(c => c.CreationDate).First().CreationDate;

                var currentWallet = user.Wallets.OrderByDescending(c => c.CreationDate).First();
                result.TotalWalletsWithAuc += currentWallet.AUCBalance > 0 ? 1 : 0;
                result.AucHolded += Convert.ToDouble(currentWallet.AUCBalance ?? 0);
                result.AucHoldedInProgress += user.ReferralStatusType == ReferralStatusType.InProgress ? Convert.ToDouble(currentWallet.AUCBalance ?? 0) : 0;
            }
            result.UsersConfirmed = confirmedUsers.GroupBy(c => c.Value.ToString("yyyy-MM-dd"))
                .Select(g => new DashboardResponse.RegistrationData() { Date = DateTime.Parse(g.Key), Value = g.Count() }).OrderBy(c => c.Date).ToList();
            result.UsersConfirmedLastSitutation = GetFlagData(result.UsersConfirmed);

            result.AucRatioPerConfirmedUser = result.TotalUsersConfirmed > 0 ? result.AucHolded / result.TotalUsersConfirmed : 0;
            result.AucRatioPerUserInProgress = result.TotalWalletsInProgress > 0 ? result.AucHoldedInProgress / result.TotalWalletsInProgress : 0;

            result.Following.Add(new DashboardResponse.DistributionData() { Name = "Asset", Amount = assetFollowers.Result.Count });
            result.Following.Add(new DashboardResponse.DistributionData() { Name = "Advisor", Amount = advisorFollowers.Result.Count });

            result.ReferralStatus = users.Result.Where(c => c.ReferralStatus.HasValue).GroupBy(c => c.ReferralStatus.Value)
                .Select(g => new DashboardResponse.DistributionData() { Name = ReferralStatusType.Get(g.Key).GetDescription(), Amount = g.Count() }).ToList();

            result.Advices = advices.Result.GroupBy(c => c.AssetId)
                .Select(g => new DashboardResponse.DistributionData() { Name = assets.First(a => a.Id == g.Key).Code, Amount = g.Count() }).ToList();

            var groupedFollowers = advisorFollowers.Result.GroupBy(c => c.AdvisorId).Select(g => new { Id = g.Key, Value = g.Count() }).OrderByDescending(c => c.Value);
            if (groupedFollowers.Any())
            {
                groupedFollowers = groupedFollowers.Take(groupedFollowers.Count() > 5 ? 5 : groupedFollowers.Count()).OrderByDescending(c => c.Value);
                var consideredFollowers = advisorFollowers.Result.Where(c => groupedFollowers.Any(a => a.Id == c.AdvisorId));
                result.AdvisorFollowers = groupedFollowers.Select(c => new DashboardResponse.AdvisorData()
                {
                    Id = c.Id,
                    Name = advisors.First(a => a.Id == c.Id).Name,
                    Total = c.Value,
                    SubValue1 = consideredFollowers.Count(a => a.CreationDate >= cutDayForActivity)
                }).ToList();
            }

            var groupedAdvices = advices.Result.GroupBy(c => c.AdvisorId).Select(g => new { Id = g.Key, Value = g.Count() }).OrderByDescending(c => c.Value);
            if (groupedAdvices.Any())
            {
                groupedAdvices = groupedAdvices.Take(groupedAdvices.Count() > 5 ? 5 : groupedAdvices.Count()).OrderByDescending(c => c.Value);
                var consideredAdvices = advices.Result.Where(c => groupedAdvices.Any(a => a.Id == c.AdvisorId));
                result.AdvisorAdvices = groupedAdvices.Select(c => new DashboardResponse.AdvisorData()
                {
                    Id = c.Id,
                    Name = advisors.First(a => a.Id == c.Id).Name,
                    Total = c.Value,
                    SubValue1 = consideredAdvices.Count(a => a.CreationDate >= cutDayForActivity)
                }).ToList();
            }

            var groupedReferred = users.Result.Where(c => c.ReferralStatus.HasValue).GroupBy(c => c.ReferredId.Value)
                .Select(g => new { Id = g.Key, Value = g.Count() }).OrderByDescending(c => c.Value);
            if (groupedReferred.Any())
            {
                groupedReferred = groupedReferred.Take(groupedReferred.Count() > 10 ? 10 : groupedReferred.Count()).OrderByDescending(c => c.Value);
                var consideredReferred= users.Result.Where(c => c.ReferralStatus.HasValue).Where(c => groupedReferred.Any(a => a.Id == c.Id));
                result.AdvisorReferral = groupedReferred.Select(c => new DashboardResponse.AdvisorData()
                {
                    Id = c.Id,
                    Name = advisors.First(a => a.Id == c.Id).Name,
                    Total = c.Value,
                    SubValue1 = consideredReferred.Count(a => a.Id == c.Id && a.ReferralStatusType == ReferralStatusType.InProgress),
                    SubValue2 = consideredReferred.Count(a => a.Id == c.Id && a.ReferralStatusType == ReferralStatusType.Interrupted),
                    SubValue3 = consideredReferred.Count(a => a.Id == c.Id && (a.ReferralStatusType == ReferralStatusType.Finished || a.ReferralStatusType == ReferralStatusType.Paid))
                }).ToList();
            }

            return result;
        }

        private DashboardResponse.FlagData GetFlagData(List<DashboardResponse.RegistrationData> registrationDatas)
        {
            if (!registrationDatas.Any())
                return null;
            else
            {
                var last = registrationDatas.Last();
                return new DashboardResponse.FlagData()
                {
                    Date = last.Date,
                    Description = registrationDatas.Count == 1 ? GetFlagDescription(last.Value) : GetFlagDescription(last.Value - registrationDatas.ElementAt(registrationDatas.Count - 2).Value)
                };
            }
        }

        private string GetFlagDescription(int value)
        {
            return value == 0 ? "zero" : value > 0 ? $"+{value}" : $"-{value}";
        }
    }
}
