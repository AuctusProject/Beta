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
    public class FacebookApi : ApiBase, IFacebookApi
    {
        private const string GET_USER_ADDRESS = "me?fields=id,name,email,picture&access_token=";
        private const string DEBUG_TOKEN_ADDRESS = "debug_token?input_token={0}&access_token={1}|{2}";
        protected readonly IConfigurationRoot Configuration;
        public FacebookApi(IConfigurationRoot configuration) : base("https://graph.facebook.com/")
        {
            Configuration = configuration;
        }

        public SocialUser GetSocialUser(string accessToken)
        {
            ValidateAccessToken(accessToken);
            var responseContent = GetWithRetry(GET_USER_ADDRESS + accessToken);
            return JsonConvert.DeserializeObject<SocialUser>(responseContent);
        }

        private void ValidateAccessToken(string accessToken)
        {
            var appId = Configuration.GetSection("Auth:FacebookAppID").Get<string>();
            var appSecret = Configuration.GetSection("Auth:FacebookAppSecret").Get<string>();
            var responseContent = GetWithRetry(String.Format(DEBUG_TOKEN_ADDRESS, accessToken, appId, appSecret));
            var debugToken = JsonConvert.DeserializeObject<DebugToken>(responseContent);
            if(debugToken == null || debugToken.Data == null || debugToken.Data.AppId != appId || !debugToken.Data.IsValid)
            {
                throw new ArgumentException("Invalid access token");
            }
        }
    }
}
