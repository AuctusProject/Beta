﻿using Auctus.DataAccess.Account;
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
        public UserBusiness(ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(loggerFactory, cache, email, ip) { }

        public User GetByEmail(string email)
        {
            return Data.GetByEmail(email);
        }

        public LoginResponse Login(string email, string password)
        {
            BaseEmailValidation(email);
            EmailValidation(email);
            BasePasswordValidation(password);

            var user = Data.GetForLogin(email);
            if (user == null)
                throw new ArgumentException("Email is invalid.");
            else if (user.Password != Security.Hash(password))
                throw new ArgumentException("Password is invalid.");

            bool hasInvestment = true;
            decimal? aucAmount = null;
            if (!user.IsAdvisor)
            {
                aucAmount = WalletBusiness.GetAucAmount(user.Wallet?.Address);
                hasInvestment = aucAmount >= Config.MINUMIM_AUC_TO_LOGIN;
            }
            ActionBusiness.InsertNewLogin(user.Id, aucAmount);
            return new Model.LoginResponse()
            {
                Email = user.Email,
                PendingConfirmation = !user.ConfirmationDate.HasValue,
                IsAdvisor = user.IsAdvisor,
                HasInvestment = hasInvestment,
                ResquestedToBeAdvisor = user.RequestToBeAdvisor != null
            };
        }

        public async Task<LoginResponse> SimpleRegister(string email, string password, bool requestedToBeAdvisor)
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

            return new Model.LoginResponse()
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

        public LoginResponse ConfirmEmail(string code)
        {
            var user = Data.GetByConfirmationCode(code);
            if (user == null)
                throw new ArgumentException("Invalid confirmation code.");

            user.ConfirmationDate = DateTime.UtcNow;
            Data.Update(user);

            return new Model.LoginResponse()
            {
                Email = user.Email,
                HasInvestment = false,
                PendingConfirmation = false,
                IsAdvisor = false,
                ResquestedToBeAdvisor = user.RequestToBeAdvisor != null
            };
        }

        public LoginResponse ValidateSignature(string address, string signature)
        {
            BaseEmailValidation(LoggedEmail);
            var user = Data.GetForNewWallet(LoggedEmail);
            if (user == null)
                throw new ArgumentException("User cannot be found.");
            if (!user.ConfirmationDate.HasValue)
                throw new ArgumentException("Email was not confirmed.");
            if (string.IsNullOrWhiteSpace(signature))
                throw new ArgumentException("Signature cannot be empty.");

            address = WalletBusiness.GetAddressFormatted(address);

            var wallet = WalletBusiness.GetByAddress(address);
            if (wallet != null)
            {
                if (wallet.UserId == user.Id)
                    throw new ArgumentException("The wallet is already linked to your account.");
                else
                    throw new ArgumentException("The wallet is already on used.");
            }

            var message = $"{address} is my address.\n{DateTime.Today.Year}-{DateTime.Today.Month.ToString().PadLeft(2, '0')}-{DateTime.Today.Day.ToString().PadLeft(2, '0')}";
            var recoveryAddress = Signature.HashAndEcRecover(message, signature)?.ToLower();
            if (address != recoveryAddress)
                throw new ArgumentException("Invalid signature.");

            decimal? aucAmount = null;
            if (!user.IsAdvisor)
            {
                aucAmount = WalletBusiness.GetAucAmount(address);
                if (aucAmount < Config.MINUMIM_AUC_TO_LOGIN)
                    throw new UnauthorizedAccessException("Wallet does not have enough AUC.");
            }

            var creationDate = DateTime.UtcNow;
            WalletBusiness.InsertNew(creationDate, user.Id, address);
            ActionBusiness.InsertNewWallet(creationDate, user.Id, $"Message: {message} --- Signature: {signature}", aucAmount ?? null);

            return new LoginResponse()
            {
                Email = user.Email,
                HasInvestment = true,
                IsAdvisor = user.IsAdvisor,
                PendingConfirmation = false,
                ResquestedToBeAdvisor = user.RequestToBeAdvisor != null
            };
        }

        public void ChangePassword(string currentPassword, string newPassword)
        {
            var user = GetValidUser();
            if (user.Password != Security.Hash(currentPassword))
                throw new ArgumentException("Current password is incorrect.");

            BasePasswordValidation(newPassword);
            PasswordValidation(newPassword);

            user.Password = Security.Hash(newPassword);
            Data.Update(user);
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

        public static void BaseEmailValidation(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email must be filled.");
        }

        public static void EmailValidation(string email)
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
