using Auctus.Business.Web3;
using Auctus.DataAccess.Account;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using Auctus.Model;
using Auctus.Util;
using Auctus.Util.NotShared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Auctus.Business.Account
{
    public class WalletBusiness : BaseBusiness<Wallet, IWalletData<Wallet>>
    {
        public WalletBusiness(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(serviceProvider, loggerFactory, cache, email, ip) { }

        public void ValidateUserWallet(User user)
        {
            var cacheKey = user.Email.ToLower().Trim() + "validated";
            var validated = MemoryCache.Get<object>(cacheKey);
            if (validated == null)
            {
                var wallet = Data.GetByUser(user.Id);
                if (wallet == null)
                    throw new UnauthorizedAccessException("Wallet was not defined.");

                var aucAmount = GetAucAmount(wallet.Address);
                if (aucAmount < Config.MINUMIM_AUC_TO_LOGIN)
                    throw new UnauthorizedAccessException("Wallet does not have enough AUC.");

                MemoryCache.Set<object>(cacheKey, true, 10);
                if(aucAmount.HasValue)
                    ActionBusiness.InsertNewAucVerification(user.Id, aucAmount.Value);
            }
        }

        public decimal? GetAucAmount(string address)
        {
            if (string.IsNullOrEmpty(address))
                return null;

            return Web3Business.GetAucAmount(address);
        }

        public bool IsValidAddress(string address)
        {
            return Regex.IsMatch(address, "^(0x)?[0-9a-f]{40}$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

        public Wallet InsertNew(DateTime creationDate, int userId, string address)
        {
            var wallet = new Wallet()
            {
                Address = address,
                UserId = userId,
                CreationDate = creationDate
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
                throw new ArgumentException("Address cannot be empty.");

            address = address.ToLower().Trim();
            if (!IsValidAddress(address))
                throw new ArgumentException("Invalid address.");

            if (address.StartsWith("0x"))
                return address;
            else
                return "0x" + address;
        }
    }
}
