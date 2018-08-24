using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
using Auctus.DomainObjects.Asset;
using Auctus.Model;
using Auctus.Util;
using Auctus.Util.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    public class UserBusiness : BaseBusiness<User, IUserData<User>>
    {
        public UserBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public User GetByEmail(string email)
        {
            return Data.GetByEmail(email);
        }

        public User GetById(int id)
        {
            return Data.GetById(id);
        }

        public LoginResponse Login(string email, string password)
        {
            BaseEmailValidation(email);
            EmailValidation(email);
            BasePasswordValidation(password);

            var user = Data.GetForLogin(email);
            if (user == null)
                throw new BusinessException("Email is invalid.");
            else if (user.Password != GetHashedPassword(password, user.Email, user.CreationDate))
                throw new BusinessException("Password is invalid.");

            bool hasInvestment = true;
            decimal? aucAmount = null;
            if (!IsValidAdvisor(user))
            {
                aucAmount = WalletBusiness.GetAucAmount(user.Wallet?.Address);
                hasInvestment = aucAmount >= GetMinimumAucAmountForUser(user);
            }
            ActionBusiness.InsertNewLogin(user.Id, aucAmount);
            return new Model.LoginResponse()
            {
                Id = user.Id,
                Email = user.Email,
                PendingConfirmation = !user.ConfirmationDate.HasValue,
                IsAdvisor = IsValidAdvisor(user),
                HasInvestment = hasInvestment,
                RequestedToBeAdvisor = user.RequestToBeAdvisor != null
            };
        }

        public bool IsValidAdvisor(User user)
        {
            return user.IsAdvisor && ((DomainObjects.Advisor.Advisor)user).Enabled;
        }

        public decimal GetMinimumAucAmountForUser(User user)
        {
            return Convert.ToDecimal(MinimumAucLogin * (1.0 - (user.ReferredId.HasValue ? DiscountPercentageOnAuc : 0)));
        }

        public async Task<LoginResponse> Register(string email, string password, string referralCode, bool requestedToBeAdvisor)
        {
            BaseEmailValidation(email);
            EmailValidation(email);
            BasePasswordValidation(password);
            PasswordValidation(password);

            var user = Data.GetByEmail(email);
            if (user != null)
                throw new BusinessException("Email already registered.");

            var referredUser = GetReferredUser(referralCode);

            user = new User();
            user.Email = email.ToLower().Trim();
            user.CreationDate = Data.GetDateTimeNow();
            user.ConfirmationCode = Guid.NewGuid().ToString();
            user.Password = GetHashedPassword(password, user.Email, user.CreationDate);
            user.ReferralCode = GenerateReferralCode();
            user.ReferredId = referredUser?.Id;
            user.AllowNotifications = true;
            Data.Insert(user);

            await SendEmailConfirmation(user.Email, user.ConfirmationCode, requestedToBeAdvisor);

            return new LoginResponse()
            {
                Email = user.Email,
                HasInvestment = false,
                PendingConfirmation = true,
                IsAdvisor = false,
                RequestedToBeAdvisor = requestedToBeAdvisor
            };
        }

        public void SetReferralCode(string referralCode)
        {
            var user = GetByEmail(LoggedEmail);
            var referredUser = GetReferredUser(referralCode);
            user.ReferredId = referredUser.Id;
            Data.Update(user);
        }

        private string GenerateReferralCode()
        {
            User user;
            String referralCode;
            do
            {
                referralCode = Util.Util.GetRandomString(7);
                user = Data.GetByReferralCode(referralCode.ToUpper());
            } while (user != null);

            return referralCode;
        }

        private User GetReferredUser(string referralCode)
        {
            if (!string.IsNullOrWhiteSpace(referralCode))
            {
                var user = Data.GetByReferralCode(referralCode.ToUpper());

                if (user == null)
                {
                    throw new BusinessException("Invalid referral code");
                }
                return user;
            }
            return null;
        }

        private string GetHashedPassword(string password, string email, DateTime creationDate)
        {
            return Security.Hash(password, $"{email}{creationDate.Ticks}{HashSecret}");
        }

        public async Task ResendEmailConfirmation()
        {
            var email = LoggedEmail;
            BaseEmailValidation(email);
            EmailValidation(email);

            var user = Data.GetByEmail(email);
            if (user == null)
                throw new NotFoundException("User cannot be found.");

            user.ConfirmationCode = Guid.NewGuid().ToString();
            Data.Update(user);

            await SendEmailConfirmation(email, user.ConfirmationCode, false);
        }

        public LoginResponse ConfirmEmail(string code)
        {
            var user = Data.GetByConfirmationCode(code);
            if (user == null)
                throw new BusinessException("Invalid confirmation code.");

            user.ConfirmationDate = Data.GetDateTimeNow();
            Data.Update(user);

            return new Model.LoginResponse()
            {
                Email = user.Email,
                HasInvestment = false,
                PendingConfirmation = false,
                IsAdvisor = false,
                RequestedToBeAdvisor = user.RequestToBeAdvisor != null
            };
        }

        public LoginResponse ValidateSignature(string address, string signature)
        {
            BaseEmailValidation(LoggedEmail);
            var user = Data.GetForNewWallet(LoggedEmail);
            if (user == null)
                throw new NotFoundException("User cannot be found.");
            if (!user.ConfirmationDate.HasValue)
                throw new BusinessException("Email was not confirmed.");
            if (string.IsNullOrWhiteSpace(signature))
                throw new BusinessException("Signature cannot be empty.");

            address = WalletBusiness.GetAddressFormatted(address);

            var wallet = WalletBusiness.GetByAddress(address);
            if (wallet != null)
            {
                if (wallet.UserId == user.Id)
                    throw new BusinessException("The wallet is already linked to your account.");
                else
                    throw new BusinessException("The wallet is already on used.");
            }

            var message = $"I accept the Privacy Policy and Terms of Use.";
            var recoveryAddress = Signature.HashAndEcRecover(message, signature)?.ToLower();
            if (address != recoveryAddress)
                throw new BusinessException("Invalid signature.");

            decimal? aucAmount = null;
            if (!IsValidAdvisor(user))
            {
                aucAmount = WalletBusiness.GetAucAmount(address);
                WalletBusiness.ValidateAucAmount(aucAmount.Value, GetMinimumAucAmountForUser(user));
            }

            var creationDate = Data.GetDateTimeNow();
            using (var transaction = TransactionalDapperCommand)
            {
                transaction.Insert(WalletBusiness.CreateNew(creationDate, user.Id, address, aucAmount));
                if (user.ReferredId.HasValue)
                {
                    user.ReferralStatus = ReferralStatusType.InProgress.Value;
                    transaction.Update(user);
                }
                transaction.Commit();
            }
            ActionBusiness.InsertNewWallet(creationDate, user.Id, $"Message: {message} --- Signature: {signature}", aucAmount ?? null);

            return new LoginResponse()
            {
                Email = user.Email,
                HasInvestment = true,
                IsAdvisor = IsValidAdvisor(user),
                PendingConfirmation = false,
                RequestedToBeAdvisor = user.RequestToBeAdvisor != null
            };
        }

        public void ChangePassword(string currentPassword, string newPassword)
        {
            var user = GetValidUser();
            if (user.Password != GetHashedPassword(currentPassword, user.Email, user.CreationDate))
                throw new BusinessException("Current password is incorrect.");

            UpdatePassword(user, newPassword);
        }

        public void SetUsersAucSituation()
        {
            var users = Data.ListForAucSituation();
            foreach (var user in users)
            {
                var start = user.Wallets.OrderBy(c => c.CreationDate).First().CreationDate;
                var currentWallet = user.Wallets.OrderByDescending(c => c.CreationDate).First();
                currentWallet.AUCBalance = WalletBusiness.GetAucAmount(currentWallet.Address);
                ActionBusiness.InsertJobAucVerification(user.Id, currentWallet.AUCBalance.Value);
                using (var transaction = TransactionalDapperCommand)
                {
                    transaction.Update(currentWallet);
                    if (user.ReferralStatusType == ReferralStatusType.InProgress)
                    {
                        if (currentWallet.AUCBalance < GetMinimumAucAmountForUser(user))
                        {
                            user.ReferralStatus = ReferralStatusType.Interrupted.Value;
                            transaction.Update(user);
                        }
                        else if (Data.GetDateTimeNow().Subtract(start).TotalDays >= MinimumDaysToKeepAuc)
                        {
                            user.ReferralStatus = ReferralStatusType.Finished.Value;
                            transaction.Update(user);
                        }
                    }
                    transaction.Commit();
                }
            }
        }

        public void UpdatePassword(User user, string password)
        {
            BasePasswordValidation(password);
            PasswordValidation(password);

            user.Password = GetHashedPassword(password, user.Email, user.CreationDate);
            Update(user);
        }

        public new void Update(User user)
        {
            Data.Update(user);
            if (user.ConfirmationDate.HasValue)
            {
                var cacheKey = GetUserCacheKey();
                if (user.IsAdvisor)
                    MemoryCache.Set<DomainObjects.Advisor.Advisor>(cacheKey, (DomainObjects.Advisor.Advisor)user);
                else
                    MemoryCache.Set<User>(cacheKey, user);
            }
        }

        public List<User> ListAllUsersData()
        {
            return Data.ListAllUsersData();
        }

        private async Task SendEmailConfirmation(string email, string code, bool requestedToBeAdvisor)
        {
            await EmailBusiness.SendAsync(new string[] { email },
                "Verify your email address - Auctus Beta",
                string.Format(@"Hello,
<br/><br/>
To activate your account please verify your email address and complete your registration <a href='{0}/confirm?c={1}{2}' target='_blank'>click here</a>.
<br/><br/>
<small>If you didn’t ask to verify this address, you can ignore this email.</small>
<br/><br/>
Thanks,
<br/>
Auctus Team", WebUrl, code, requestedToBeAdvisor ? "&a=" : ""));
        }

        public static void BaseEmailValidation(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new BusinessException("Email must be filled.");
        }

        public void EmailValidation(string email)
        {
            if (!EmailBusiness.IsValidEmail(email))
                throw new BusinessException("Email informed is invalid.");
        }

        private void BasePasswordValidation(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new BusinessException("Password must be filled.");
        }

        private void PasswordValidation(string password)
        {
            if (password.Length < 8)
                throw new BusinessException("Password must be at least 8 characters.");
            if (password.Length > 100)
                throw new BusinessException("Password cannot have more than 100 characters.");
            if (password.Contains(" "))
                throw new BusinessException("Password cannot have spaces.");
        }

        public FollowAdvisor FollowUnfollowAdvisor(int advisorId, FollowActionType followActionType)
        {
            var user = GetValidUser();

            return FollowAdvisorBusiness.Create(user.Id, advisorId, followActionType);
        }

        public FollowAsset FollowUnfollowAsset(int AssetId, FollowActionType followActionType)
        {
            var user = GetValidUser();

            return FollowAssetBusiness.Create(user.Id, AssetId, followActionType);
        }

        public ReferralProgramInfoResponse GetReferralProgramInfo()
        {
            var user = GetValidUser();
            var referredUsers = Data.ListReferredUsers(user.Id);
            ReferralProgramInfoResponse response = ConvertReferredUsersToReferralProgramInfo(referredUsers);
            response.ReferralCode = user.ReferralCode;
            return response;
        }

        public void SetConfiguration(bool allowNotifications)
        {
            var user = GetValidUser();
            user.AllowNotifications = allowNotifications;
            Update(user);
        }

        public UserConfigurationResponse GetConfiguration()
        {
            var user = GetValidUser();
            var wallet = WalletBusiness.GetByUser(user.Id);
            return new UserConfigurationResponse()
            {
                AllowNotifications = user.AllowNotifications,
                Wallet = wallet?.Address
            };
        }

        private static ReferralProgramInfoResponse ConvertReferredUsersToReferralProgramInfo(List<User> referredUsers)
        {
            var response = new ReferralProgramInfoResponse();
            foreach (var group in referredUsers.GroupBy(c => c.ReferralStatus))
            {
                if (group.Key == ReferralStatusType.InProgress.Value)
                    response.InProgressCount = group.Count();
                else if (group.Key == ReferralStatusType.Interrupted.Value)
                    response.InterruptedCount = group.Count();
                else if (group.Key == ReferralStatusType.Finished.Value)
                    response.FinishedCount = group.Count();
                else if (group.Key == ReferralStatusType.Paid.Value)
                    response.PaidCount = group.Count();
                else
                    response.NotStartedCount = group.Count();
            }

            return response;
        }

        public List<User> ListUsersFollowingAdvisorOrAsset(int advisorId, int assetId)
        {
            return Data.ListUsersFollowingAdvisorOrAsset(advisorId, assetId);
        }

        public SearchResponse Search(string searchTerm)
        {
            IEnumerable<DomainObjects.Advisor.Advisor> advisors = null;
            IEnumerable<DomainObjects.Asset.Asset> assets = null;

            Parallel.Invoke(() => advisors = AdvisorBusiness.ListByName(searchTerm), () => assets = AssetBusiness.ListByNameOrCode(searchTerm));

            var response = new SearchResponse();

            foreach(DomainObjects.Advisor.Advisor advisor in advisors)
            {
                response.Advisors.Add(new SearchResponse.AdvisorResult()
                {
                    AdvisorId = advisor.Id,
                    Description = advisor.Description,
                    Enabled = advisor.Enabled,
                    Name = advisor.Name
                });
            }
            foreach (DomainObjects.Asset.Asset asset in assets)
            {
                response.Assets.Add(new SearchResponse.AssetResult()
                {
                    AssetId = asset.Id,
                    Code = asset.Code,
                    HasAdvice = asset.HasAdvice,
                    Name = asset.Name
                });
            }

            return response;
        }
    }
}
