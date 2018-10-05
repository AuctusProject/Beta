using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Event
{
    public class CoinMarketCalResult
    {
        [JsonProperty("_metadata")]
        public MetaData MetaDataResult { get; set; }
        [JsonProperty("records")]
        public List<Record> Records { get; set; } = new List<Record>();

        public class Record
        {
            [JsonProperty("id")]
            public int Id { get; set; }
            [JsonProperty("title")]
            public string Title { get; set; }
            [JsonProperty("description")]
            public string Description { get; set; }
            [JsonProperty("date_event")]
            public DateTime EventDate { get; set; }
            [JsonProperty("created_date")]
            public DateTime CreatedDate { get; set; }
            [JsonProperty("is_hot")]
            public bool IsHot { get; set; }
            [JsonProperty("can_occur_before")]
            public bool CanOccurBefore { get; set; }
            [JsonProperty("tip_symbol")]
            public string TipSymbol { get; set; }
            [JsonProperty("tip_adress")]
            public string TipAddress { get; set; }
            [JsonProperty("twitter_account")]
            public string TwitterAccount { get; set; }
            [JsonProperty("proof")]
            public string Proof { get; set; }
            [JsonProperty("source")]
            public string Source { get; set; }
            [JsonProperty("vote_count")]
            public int VoteCount { get; set; }
            [JsonProperty("positive_vote_count")]
            public int PositiveVoteCount { get; set; }
            [JsonProperty("percentage")]
            public double Percentage { get; set; }
            [JsonProperty("coins")]
            public List<Coin> Coins { get; set; } = new List<Coin>();
            [JsonProperty("categories")]
            public List<Category> Categories { get; set; } = new List<Category>();
        }

        public class Coin
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("symbol")]
            public string Symbol { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class Category
        {
            [JsonProperty("id")]
            public int Id { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class MetaData
        {
            [JsonProperty("page")]
            public int Page { get; set; }
            [JsonProperty("max")]
            public int Maximum { get; set; }
            [JsonProperty("total_count")]
            public int TotalCount { get; set; }
            [JsonProperty("page_count")]
            public int PageCount { get; set; }
        }

        public class Auth
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
            [JsonProperty("expires_in")]
            public int ExpiresInSeconds { get; set; }
            [JsonProperty("token_type")]
            public string TokenType { get; set; }
        }
    }
}
