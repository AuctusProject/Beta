using Auctus.DataAccess.Account;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using Auctus.Model;
using Auctus.Util;
using Auctus.Util.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.Business.Account
{
    public class PasswordRecoveryBusiness : BaseBusiness<PasswordRecovery, IPasswordRecoveryData<PasswordRecovery>>
    {
        public PasswordRecoveryBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public async Task SendEmailForForgottenPasswordAsync(string email)
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

                await SendForgottenPasswordAsync(email, recovery.Token);
            }
        }

        private async Task SendForgottenPasswordAsync(string email, string code)
        {
            await EmailBusiness.SendUsingTemplateAsync(new string[] { email },
                "Reset your password - Auctus Experts",
                string.Format(@"<p>You told us you forgot your password. If you really did, <a href='{0}?resetpassword=true&c={1}' target='_blank'>click here</a> to choose a new one.</p>
                    <p style=""font-size: 12px; font-style: italic;"">If you didn't mean to reset your password, then you can just ignore this email. Your password will not change.</p>", 
                    WebUrl, code), EmailTemplate.NotificationType.ResetPassword);
        }

        public LoginResponse RecoverPassword(string code, string password)
        {
            var recovery = Data.Get(code);
            if (recovery == null)
                throw new BusinessException("There is no request for recover password.");
            if (Data.GetDateTimeNow() > recovery.CreationDate.AddMinutes(60))
                throw new BusinessException("Recover password code is expired.");

            var user = UserBusiness.GetForLoginById(recovery.UserId);
            UserBusiness.UpdatePassword(user, password);

            bool hasInvestment = UserBusiness.GetUserHasInvestment(user, out decimal? aucAmount);
            return new LoginResponse()
            {
                Id = user.Id,
                Email = user.Email,
                PendingConfirmation = !user.ConfirmationDate.HasValue,
                IsAdvisor = UserBusiness.IsValidAdvisor(user),
                HasInvestment = hasInvestment,
                RequestedToBeAdvisor = user.RequestToBeAdvisor != null
            };
        }
    }
}
