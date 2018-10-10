using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Event;
using Auctus.DomainObjects.Event;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Auctus.DomainObjects.Event.CoinMarketCalResult;

namespace Auctus.DataAccess.Event
{
    public class CoinMarketCalApi : ApiBase, ICoinMarketCalApi
    {
        private readonly string ClientId;
        private readonly string ClientSecret;
        private readonly Cache MemoryCache;

        private const string AUTHORIZATION_ROUTE = "oauth/v2/token";
        private const string EVENTS_ROUTE = "v1/events";
        private const string COINS_ROUTE = "v1/coins";
        private const string CATEGORIES_ROUTE = "v1/categories";

        public CoinMarketCalApi(IConfigurationRoot configuration, Cache cache) : base("https://api.coinmarketcal.com")
        {
            ClientId = configuration.GetSection("Auth:CoinMarketCalClientID").Get<string>();
            ClientSecret = configuration.GetSection("Auth:CoinMarketCalClientSecret").Get<string>();
            MemoryCache = cache;
        }

        public List<Coin> ListCoins()
        {
            var responseContent = GetWithRetry(GetRoute(COINS_ROUTE));
            return JsonConvert.DeserializeObject<Coin[]>(responseContent).ToList();
        }

        public List<Category> ListCategories()
        {
            var responseContent = GetWithRetry(GetRoute(CATEGORIES_ROUTE));
            return JsonConvert.DeserializeObject<Category[]>(responseContent).ToList();
        }

        public CoinMarketCalResult ListEvents(int page = 1, int limit = 100, DateTime? startDate = null, DateTime? endDate = null)
        {
            var route = GetRoute(EVENTS_ROUTE) + $"&showMetadata=true&page={page}&max={limit}";
            if (startDate.HasValue)
                route += $"&dateRangeStart={GetFormattedDateToQuery(startDate.Value)}";
            if (endDate.HasValue)
                route += $"&dateRangeEnd={GetFormattedDateToQuery(endDate.Value)}";

            var responseContent = GetWithRetry(route);
            return JsonConvert.DeserializeObject<CoinMarketCalResult>(responseContent);
        }

        private string GetFormattedDateToQuery(DateTime dateTime)
        {
            return $"{dateTime.Day.ToString().PadLeft(2, '0')}%2F{dateTime.Month.ToString().PadLeft(2, '0')}%2F{dateTime.Year}";
        }

        private string GetRoute(string route)
        {
            string cacheKey = "CoinMarKetCalAuthorization";
            var authorization = MemoryCache.Get<Auth>(cacheKey);
            if (authorization == null)
            {
                var responseContent = GetWithRetry(AUTHORIZATION_ROUTE + $"?grant_type=client_credentials&client_id={ClientId}&client_secret={ClientSecret}");
                authorization = JsonConvert.DeserializeObject<Auth>(responseContent);
                if (authorization != null)
                {
                    var timeout = 1440;
                    if (authorization.ExpiresInSeconds > 0)
                        timeout = (authorization.ExpiresInSeconds / 60) - 1440;

                    MemoryCache.Set<Auth>(cacheKey, authorization, timeout);
                }
            }
            return route + "?access_token=" + authorization.AccessToken;
        }
    }
}
