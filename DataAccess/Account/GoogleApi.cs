using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Auctus.DataAccess.Account
{
    public class GoogleApi : ApiBase, IGoogleApi
    {
        private const string GET_USER_ADDRESS = "oauth2/v3/tokeninfo?access_token=";
        protected readonly IConfigurationRoot Configuration;
        public GoogleApi(IConfigurationRoot configuration) : base("https://www.googleapis.com/")
        {
            Configuration = configuration;
        }

        public SocialUser GetSocialUser(string accessToken)
        {
            var responseContent = GetWithRetry(GET_USER_ADDRESS + accessToken);
            var googleSocialUser = JsonConvert.DeserializeObject<GoogleSocialUser>(responseContent);
            ValidateAccessToken(googleSocialUser.GoogleClientID);
            return googleSocialUser;
        }

        private void ValidateAccessToken(String projectClientID)
        {
            var googleClientID = Configuration.GetSection("Auth:GoogleClientID").Get<string>();
            if (String.IsNullOrEmpty(projectClientID) || projectClientID != googleClientID)
            {
                throw new ArgumentException("Invalid access token");
            }
        }

        private class GoogleSocialUser : SocialUser
        {
            [JsonProperty("azp")]
            public string GoogleClientID { get; set; }
        }
    }
}
