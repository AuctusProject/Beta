using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using System;
using System.Collections.Generic;
using System.Text;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Auctus.Business.Account
{
    public class EarlyAccessEmailBusiness : BaseBusiness<EarlyAccessEmail, IEarlyAccessEmailData<EarlyAccessEmail>>
    {
        public EarlyAccessEmailBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip)
        {
        }

        public void Create(string name, string email, string twitter)
        {
            UserBusiness.EmailValidation(email);

            var previousRecord = Data.GetByEmail(email);

            if (!string.IsNullOrWhiteSpace(name) && name.Length > 50)
                name = name.Substring(0, 50);
            if (!string.IsNullOrWhiteSpace(twitter) && twitter.Length > 15)
                twitter = twitter.Substring(0, 15);

            if (previousRecord == null)
            {
                Insert(new EarlyAccessEmail()
                {
                    CreationDate = Data.GetDateTimeNow(),
                    Email = email,
                    Name = name,
                    Twitter = twitter
                });
            }
            else
            {
                previousRecord.Name = name;
                previousRecord.Twitter = twitter;
                Update(previousRecord);
            }
        }
    }
}
