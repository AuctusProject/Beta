using Auctus.DataAccess.Account;
using Auctus.DataAccess.Core;
using Auctus.DataAccess.Exchanges;
using Auctus.DomainObjects.Account;
using Auctus.Model;
using Auctus.Util;
using Auctus.Util.NotShared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Auctus.Business.Account
{
    public class UserBusiness : BaseBusiness<User, UserData>
    {
        public UserBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }

        public Login Login(string email, string password)
        {
            BaseEmailValidation(email);
            EmailValidation(email);
            BasePasswordValidation(password);

            var user = Data.GetForLogin(email);
            if (user == null)
                throw new ArgumentException("Email is invalid.");
            else if (user.Password != Security.Hash(password))
                throw new ArgumentException("Password is invalid.");

            var hasInvestment = user.IsAdvisor || WalletBusiness.IsValidAucAmount(user.Wallet?.Address);
            return new Model.Login()
            {
                Email = user.Email,
                PendingConfirmation = !user.ConfirmationDate.HasValue,
                IsAdvisor = user.IsAdvisor,
                HasInvestment = hasInvestment,
                ResquestedToBeAdvisor = user.RequestToBeAdvisor != null
            };
        }

        public async Task<Login> SimpleRegister(string email, string password, bool requestedToBeAdvisor)
        {
            BaseEmailValidation(email);
            EmailValidation(email);
            BasePasswordValidation(password);
            PasswordValidation(password);

            var user = Data.GetByEmail(email);
            if (user != null)
                throw new ArgumentException("Email already registered.");

            user = new User();
            user.Email = email.ToLower().Trim();
            user.CreationDate = DateTime.UtcNow;
            user.Password = Security.Hash(password);
            user.ConfirmationCode = Guid.NewGuid().ToString();
            Data.Insert(user);

            await SendEmailConfirmation(user.Email, user.ConfirmationCode, requestedToBeAdvisor);

            return new Model.Login()
            {
                Email = user.Email,
                HasInvestment = false,
                PendingConfirmation = true,
                IsAdvisor = false,
                ResquestedToBeAdvisor = requestedToBeAdvisor
            };
        }

        public async Task ResendEmailConfirmation(string email)
        {
            BaseEmailValidation(email);
            EmailValidation(email);

            var user = Data.GetByEmail(email);
            if (user == null)
                throw new ArgumentException("User cannot be found.");

            user.ConfirmationCode = Guid.NewGuid().ToString();
            Data.Update(user);

            await SendEmailConfirmation(email, user.ConfirmationCode, false);
        }

        public Login ConfirmEmail(string code)
        {
            var user = Data.GetByConfirmationCode(code);
            if (user == null)
                throw new ArgumentException("Invalid confirmation code.");

            user.ConfirmationDate = DateTime.UtcNow;
            Data.Update(user);

            return new Model.Login()
            {
                Email = user.Email,
                HasInvestment = false,
                PendingConfirmation = false,
                IsAdvisor = false,
                ResquestedToBeAdvisor = user.RequestToBeAdvisor != null
            };
        }

        public void ChangePassword(string email, string currentPassword, string newPassword)
        {
            var user = GetValidUser(email);
            if (user.Password != Security.Hash(currentPassword))
                throw new ArgumentException("Current password is incorrect.");

            BasePasswordValidation(newPassword);
            PasswordValidation(newPassword);

            user.Password = Security.Hash(newPassword);
            Data.Update(user);
        }

        public User GetValidUser(string email)
        {
            BaseEmailValidation(email);
            var cacheKey = email.ToLower().Trim();
            var user = MemoryCache.Get<User>(cacheKey);
            if (user == null)
            {
                EmailValidation(email);
                user = Data.GetByEmail(email);
                if (user == null)
                    throw new ArgumentException("User cannot be found.");
                if (!user.ConfirmationDate.HasValue)
                    throw new ArgumentException("Email was not confirmed.");

                if (!user.IsAdvisor)
                    WalletBusiness.ValidateUserWallet(user);

                MemoryCache.Set<User>(cacheKey, user);
                return user;
            }
            else
            {
                if (!user.IsAdvisor)
                    WalletBusiness.ValidateUserWallet(user);
                return user;
            }
        }

        private async Task SendEmailConfirmation(string email, string code, bool requestedToBeAdvisor)
        {
            await Email.SendAsync(
                new string[] { email },
                "Verify your email address - Auctus Beta",
                string.Format(@"Hello,
<br/><br/>
To activate your account please verify your email address and complete your registration <a href='{0}/confirm?c={1}{2}' target='_blank'>click here</a>.
<br/><br/>
<small>If you didn’t ask to verify this address, you can ignore this email.</small>
<br/><br/>
Thanks,
<br/>
Auctus Team", Config.WEB_URL, code, requestedToBeAdvisor ? "&a=" : ""));
        }

        private void BaseEmailValidation(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email must be filled.");
        }

        private void EmailValidation(string email)
        {
            if (!Email.IsValidEmail(email))
                throw new ArgumentException("Email informed is invalid.");
        }

        private void BasePasswordValidation(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password must be filled.");
        }

        private void PasswordValidation(string password)
        {
            if (password.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters.");
            if (password.Length > 100)
                throw new ArgumentException("Password cannot have more than 100 characters.");
            if (password.Contains(" "))
                throw new ArgumentException("Password cannot have spaces.");
        }
    }
}
