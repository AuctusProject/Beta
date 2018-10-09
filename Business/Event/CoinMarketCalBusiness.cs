using Auctus.DataAccessInterfaces.Event;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using static Auctus.DomainObjects.Event.CoinMarketCalResult;

namespace Auctus.Business.Event
{
    public class CoinMarketCalBusiness
    {
        private readonly ICoinMarketCalApi Api;

        internal CoinMarketCalBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider)
        {
            Api = (ICoinMarketCalApi)serviceProvider.GetService(typeof(ICoinMarketCalApi));
        }

        public List<Category> ListCategories()
        {
            return Api.ListCategories();
        }

        public List<Coin> ListCoins()
        {
            return Api.ListCoins();
        }

        public List<Record> ListEvents(DateTime? startDate = null, DateTime? endDate = null)
        {
            var page = 1;
            var limit = 100;
            var result = new List<Record>();
            while (true)
            {
                var response = Api.ListEvents(page, limit, startDate, endDate);
                if (response == null || response.Records == null || !response.Records.Any())
                    break;

                result.AddRange(response.Records);
                ++page;
                if (response.MetaDataResult == null || page > response.MetaDataResult.PageCount)
                    break;
            }
            return result.OrderByDescending(c => c.FormattedEventDate).ThenByDescending(c => c.FormattedCreatedDate).ToList();
        }
    }
}
