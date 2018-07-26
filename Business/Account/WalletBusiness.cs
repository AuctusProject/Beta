using Auctus.DataAccess.Account;
using Auctus.DomainObjects.Account;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Auctus.Business.Account
{
    public class WalletBusiness : BaseBusiness<Wallet, WalletData>
    {
        public WalletBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }

        public void ValidateUserWallet(User user)
        {
            var cacheKey = user.Email.ToLower().Trim() + "validated";
            var validated = MemoryCache.Get<object>(cacheKey);
            if (validated == null)
            {
                var wallet = Data.GetByUser(user.Id);
                if (wallet == null)
                    throw new ArgumentException("Wallet was not defined.");
                if (!IsValidAucAmount(wallet.Address))
                    throw new ArgumentException("Wallet does not have enough AUC.");

                MemoryCache.Set<object>(cacheKey, true, 10);
            }
        }

        public bool IsValidAucAmount(string address)
        {
            if (string.IsNullOrEmpty(address))
                return false;

            //TODO: check AUC amount
            return true;
        }

        private bool IsValidAddress(string address)
        {
            return Regex.IsMatch(address, "^(0x)?[0-9a-f]{40}$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

        private string GetAddressFormatted(string address)
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
