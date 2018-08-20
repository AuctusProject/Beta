using Auctus.DataAccess.Account;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using Auctus.Model;
using Auctus.Util;
using Auctus.Util.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Auctus.Business.Account
{
    public class WalletBusiness : BaseBusiness<Wallet, IWalletData<Wallet>>
    {
        public WalletBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, loggerFactory, cache, email, ip) { }

        public void ValidateUserWallet(User user)
        {
            var cacheKey = user.Email.ToLower().Trim() + "validated";
            var validated = MemoryCache.Get<object>(cacheKey);
            if (validated == null)
            {
                var wallet = Data.GetByUser(user.Id);
                if (wallet == null)
                    throw new NotFoundException("Wallet was not defined.");

                var aucAmount = GetAucAmount(wallet.Address);
                ActionBusiness.InsertNewAucVerification(user.Id, aucAmount);

                if (aucAmount < MinimumAucLogin)
                    throw new UnauthorizedException($"Wallet does not have enough AUC. Missing {MinimumAucLogin - aucAmount} AUCs.");

                MemoryCache.Set<object>(cacheKey, true, 10);
            }
        }

        public decimal GetAucAmount(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException("address");

            return Web3Business.GetAucAmount(address);
        }

        public bool IsValidAddress(string address)
        {
            return Regex.IsMatch(address, "^(0x)?[0-9a-f]{40}$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

        public Wallet InsertNew(DateTime creationDate, int userId, string address, decimal? aucAmount)
        {
            var wallet = new Wallet()
            {
                Address = address,
                UserId = userId,
                CreationDate = creationDate,
                AUCBalance = aucAmount
            };
            Data.Insert(wallet);
            return wallet;
        }

        public Wallet GetByAddress(string address)
        {
            return Data.GetByAddress(address);
        }

        public string GetAddressFormatted(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new BusinessException("Address cannot be empty.");

            address = address.ToLower().Trim();
            if (!IsValidAddress(address))
                throw new BusinessException("Invalid address.");

            if (address.StartsWith("0x"))
                return address;
            else
                return "0x" + address;
        }
    }
}
