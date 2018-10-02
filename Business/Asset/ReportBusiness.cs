using Auctus.DataAccessInterfaces.Asset;
using Auctus.DomainObjects.Asset;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auctus.Business.Asset
{
    public class ReportBusiness : BaseBusiness<Report, IReportData<Report>>
    {
        public ReportBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public List<Report> List(IEnumerable<int> assetsId, int? top = null, int? lastReportId = null)
        {
            var reports = Data.ListWithPagination(assetsId, top, lastReportId);
            var agencies = AgencyBusiness.All();
            reports.ForEach(c =>
            {
                c.Agency = agencies.First(a => a.Id == c.AgencyId);
                c.AgencyRating = c.Agency.AgencyRating.First(r => r.Id == c.AgencyRatingId);
            });
            return reports;
        }
    }
}
