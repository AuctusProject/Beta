using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using Auctus.DataAccessInterfaces.Email;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Auctus.DataAccess.Email
{
    public class SendGridApi : ISendGridApi
    {
        protected readonly IConfigurationRoot Configuration;
        private readonly string ApiKey;

        public SendGridApi(IConfigurationRoot configuration)
        {
            Configuration = configuration;
            ApiKey = configuration.GetSection("Auth:SendGridApiKey").Get<string>();
        }

        public async Task IncludeEmail(string email, string firstName, string lastName)
        {
            var apiKey = ApiKey;
            var client = new SendGridClient(apiKey);

            var body = @"
            [{
                'email': '{email}',
                'first_name': '{firstName}',
                'last_name': '{lastName}' 
            }]".Replace("{email}", email).Replace("{firstName}", firstName).Replace("{lastName}", lastName);

            var json = JsonConvert.DeserializeObject<Object>(body);

            var response = await client.RequestAsync(SendGridClient.Method.POST, json.ToString(), null, "contactdb/recipients");
        }
    }
}
