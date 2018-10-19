using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Advisor;
using Auctus.DomainObjects.Asset;
using Auctus.Model;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.Business.Asset
{
    public class ReportBusiness : BaseBusiness<Report, IReportData<Report>>
    {
        private const string GYRO_FINANCE = "陀螺财经";
        private const string NUMBER_CHAIN_RATING = "数链评级";
        private const string CANNON_RATING = "大炮评级";

        public ReportBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public List<Report> List(IEnumerable<int> assetsId, int? top = null, int? lastReportId = null)
        {
            var reports = Data.ListWithPagination(assetsId, top, lastReportId);
            var agencies = AgencyBusiness.All();
            reports.ForEach(c =>
            {
                c.Agency = agencies.First(a => a.Id == c.AgencyId);
                c.AgencyRating = c.AgencyRatingId.HasValue ? c.Agency.AgencyRating.First(r => r.Id == c.AgencyRatingId) : null;
            });
            return reports;
        }

        public List<RecommendationDistributionResponse> GetReportRecommendationDistribution(List<Report> reports)
        {
            var distribution = new Dictionary<int, int>();
            distribution[AdviceType.Buy.Value] = 0;
            distribution[AdviceType.Sell.Value] = 0;
            distribution[AdviceType.ClosePosition.Value] = 0;
            foreach (var report in reports)
            {
                if (report.AgencyRating != null)
                {
                    if (report.AgencyRating.HexaColor == "#3ED142")
                        distribution[AdviceType.Buy.Value]++;
                    else if (report.AgencyRating.HexaColor == "#D13E3E")
                        distribution[AdviceType.Sell.Value]++;
                    else
                        distribution[AdviceType.ClosePosition.Value]++;
                }
                else
                {
                    if (report.Score >= 3.5)
                        distribution[AdviceType.Buy.Value]++;
                    else if (report.Score >= 2.5)
                        distribution[AdviceType.ClosePosition.Value]++;
                    else
                        distribution[AdviceType.Sell.Value]++;
                }
            }
            return distribution.Where(c => c.Value > 0).Select(c => new RecommendationDistributionResponse() { Type = c.Key, Total = c.Value }).ToList();
        }

        public IEnumerable<FeedResponse> ListReports(int? top, int? lastReportId, int? assetId)
        {
            IEnumerable<int> assetsId = null;
            if (assetId.HasValue)
                assetsId = new int[] { assetId.Value };

            var reports = Task.Factory.StartNew(() => List(assetsId, top, lastReportId));
            var user = LoggedEmail != null ? UserBusiness.GetByEmail(LoggedEmail) : null;
            return UserBusiness.FillFeedList(null, reports, null, user, top, null, lastReportId, null);
        }

        public ReportResponse ConvertToReportResponse(Report report)
        {
            return new ReportResponse()
            {
                ReportId = report.Id,
                ReportDate = report.ReportDate,
                AssetId = report.AssetId,
                AgencyId = report.AgencyId,
                AgencyName = report.Agency.Name,
                AgencyWebSite = report.Agency.WebSite,
                Score = report.Score,
                Rate = report.Rate,
                RateDetails = report.AgencyRating != null ? ConvertToRateResponse(report.AgencyRating) : null,
                RateOptions = report.Agency.AgencyRating.Select(c => ConvertToRateResponse(c)).ToList()
            };
        }

        private ReportResponse.RatingDetail ConvertToRateResponse(AgencyRating agencyRating)
        {
            return new ReportResponse.RatingDetail()
            {
                Rate = agencyRating.Rate,
                HexaColor = agencyRating.HexaColor,
                Description = agencyRating.Description
            };
        }
    }
}
