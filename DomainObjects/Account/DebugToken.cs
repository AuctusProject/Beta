using Auctus.DomainObjects.Advisor;
using Auctus.Util.DapperAttributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Account
{
    public class DebugToken
    {
        public DebugTokenData Data { get; set; }

        public class DebugTokenData
        {
            [JsonProperty("app_id")]
            public string AppId { get; set; }
            [JsonProperty("is_valid")]
            public bool IsValid { get; set; }
        }
    }
}
