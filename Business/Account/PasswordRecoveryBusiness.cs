using Auctus.DataAccess.Account;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.Business.Account
{
    public class PasswordRecoveryBusiness : BaseBusiness<PasswordRecovery, IPasswordRecoveryData<PasswordRecovery>>
    {
        public PasswordRecoveryBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, loggerFactory, cache, email, ip) { }

        public async Task SendEmailForForgottenPassword(string email)
        {
            var user = UserBusiness.GetByEmail(email);
            if (user != null)
            {
                var recovery = Data.Get(user.Id);
                if (recovery == null)
                {
                    recovery = new PasswordRecovery();
                    recovery.UserId = user.Id;
                    recovery.CreationDate = Data.GetDateTimeNow();
                    recovery.Token = Guid.NewGuid().ToString();
                    Data.Insert(recovery);
                }
                else
                {
                    recovery.CreationDate = Data.GetDateTimeNow();
                    recovery.Token = Guid.NewGuid().ToString();
                    Data.Update(recovery);
                }

                await SendForgottenPassword(email, recovery.Token);
            }
        }

        private async Task SendForgottenPassword(string email, string code)
        {
            await Email.SendAsync(SendGridKey,
                new string[] { email },
                "Reset your password - Auctus Platform",
                string.Format(@"Hello,
<br/><br/>
You told us you forgot your password. If you really did, <a href='{0}/forgot-password-reset?c={1}' target='_blank'>click here</a> to choose a new one.
<br/><br/>
If you didn't mean to reset your password, then you can just ignore this email. Your password will not change.
<br/><br/>
Thanks,
<br/>
Auctus Team", WebUrl, code));
        }

        public void RecoverPassword(string code, string password)
        {
            var recovery = Data.Get(code);
            if (recovery == null)
                throw new ArgumentException("There is no request for recover password.");
            if (Data.GetDateTimeNow() > recovery.CreationDate.AddMinutes(60))
                throw new ArgumentException("Recover password code is expired.");

            UserBusiness.UpdatePassword(UserBusiness.GetById(recovery.UserId), password);
        }
    }
}
