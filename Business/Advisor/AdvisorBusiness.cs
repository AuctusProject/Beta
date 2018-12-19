using Auctus.DataAccess.Advisor;
using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Advisor;
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using Auctus.DomainObjects.Trade;
using static Auctus.Model.AdvisorPerformanceResponse;

namespace Auctus.Business.Advisor
{
    public class AdvisorBusiness : BaseBusiness<DomainObjects.Advisor.Advisor, IAdvisorData<DomainObjects.Advisor.Advisor>>
    {
        public AdvisorBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public async Task<LoginResponse> CreateAsync(string email, string password, string name, string description, string referralCode,
            bool changePicture, Stream pictureStream, string pictureExtension)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessException("Name must be filled.");
            if (name.Length > 50)
                throw new BusinessException("Name cannot have more than 50 characters.");
            if (!string.IsNullOrWhiteSpace(description) && description.Length > 160)
                throw new BusinessException("Short description cannot have more than 160 characters.");

            byte[] picture = null;
            if (changePicture && pictureStream != null)
                picture = AdvisorBusiness.GetPictureBytes(pictureStream, pictureExtension);

            User user = null;
            var updateUser = false;
            if (LoggedEmail != null)
            {
                if (!string.IsNullOrWhiteSpace(email) && email != LoggedEmail)
                    throw new BusinessException("Invalid email.");
                if (!string.IsNullOrWhiteSpace(password))
                    throw new BusinessException("Invalid password.");

                user = UserBusiness.GetForLoginByEmail(LoggedEmail);
                if (UserBusiness.IsValidAdvisor(user))
                    throw new BusinessException("User already registered.");

                if (!user.ReferredId.HasValue)
                {
                    var referredUser = UserBusiness.GetReferredUser(referralCode);
                    if (referredUser != null)
                    {
                        updateUser = true;
                        user.ReferralStatus = ReferralStatusType.InProgress.Value;
                        user.ReferredId = referredUser.Id;
                        user.BonusToReferred = UserBusiness.GetBonusToReferredUser(referredUser);
                    }
                }
                else if (!string.IsNullOrEmpty(referralCode))
                    throw new BusinessException("User already has a referral code defined.");
            }
            else
                user = UserBusiness.GetValidUserToRegister(email, password, referralCode);

            var advisorsCount = AdvisorRankingBusiness.ListAdvisorsFullData().Count;
            Guid urlGuid = Guid.NewGuid();
            var creationDate = Data.GetDateTimeNow();
            using (var transaction = TransactionalDapperCommand)
            {
                if (LoggedEmail == null)
                    transaction.Insert(user);
                else if (updateUser)
                    transaction.Update(user);

                var advisor = new DomainObjects.Advisor.Advisor()
                {
                    Id = user.Id,
                    Name = name,
                    Description = description,
                    BecameAdvisorDate = creationDate,
                    Enabled = true,
                    UrlGuid = urlGuid
                };

                transaction.Insert(advisor);

                var virtualDolar = new Order()
                {
                    AssetId = AssetUSDId,
                    CreationDate = creationDate,
                    Price = 1,
                    Quantity = VirtualMoney,
                    RemainingQuantity = VirtualMoney,
                    Status = OrderStatusType.Executed.Value,
                    StatusDate = creationDate,
                    Type = OrderType.Buy.Value,
                    UserId = user.Id,
                    ActionType = OrderActionType.Automated.Value,
                    Fee = 0
                };
                transaction.Insert(virtualDolar);

                var rating = 2.5;
                var ranking = advisorsCount + 1;
                transaction.Insert(new AdvisorRanking()
                {
                    Id = user.Id,
                    UpdateDate = creationDate,
                    Rating = rating,
                    Ranking = ranking
                });
                transaction.Insert(new AdvisorRankingHistory()
                {
                    UserId = user.Id,
                    ReferenceDate = creationDate,
                    Rating = rating,
                    Ranking = ranking
                });
                var baseProfit = AdvisorProfitBusiness.GetBaseUsdAdvisorProfit(user.Id, creationDate);
                transaction.Insert(baseProfit);
                transaction.Insert(new AdvisorProfitHistory()
                {
                    AssetId = baseProfit.AssetId,
                    OrderCount = baseProfit.OrderCount,
                    ReferenceDate = baseProfit.UpdateDate,
                    Status = baseProfit.Status,
                    SuccessCount = baseProfit.SuccessCount,
                    SummedProfitDollar = baseProfit.SummedProfitDollar,
                    SummedProfitPercentage = baseProfit.SummedProfitPercentage,
                    SummedTradeMinutes = baseProfit.SummedTradeMinutes,
                    TotalDollar = baseProfit.TotalDollar,
                    TotalQuantity = baseProfit.TotalQuantity,
                    Type = baseProfit.Type,
                    UserId = baseProfit.UserId,
                    TotalFee = baseProfit.TotalFee
                });

                transaction.Commit();               
            }

            await AzureStorageBusiness.UploadUserPictureFromBytesAsync($"{urlGuid}.png", picture ?? AdvisorBusiness.GetNoUploadedImageForAdvisor(user));
            if (LoggedEmail == null || !user.ConfirmationDate.HasValue)
                await UserBusiness.SendEmailConfirmationAsync(user.Email, user.ConfirmationCode);

            UserBusiness.ClearUserCache(user.Email);
            AdvisorRankingBusiness.AppendNewAdvisorToCache(user.Id);

            return new LoginResponse()
            {
                Id = user.Id,
                Email = user.Email,
                PendingConfirmation = !user.ConfirmationDate.HasValue,
                AdvisorName = name,
                ProfileUrlGuid = urlGuid.ToString(),
                HasInvestment = false,
                IsAdvisor = true,
                RequestedToBeAdvisor = false
            };
        }

        public async Task<Guid> EditAdvisorAsync(int id, string name, string description, bool changePicture, Stream pictureStream, string pictureExtension)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessException("Name must be filled.");
            if (name.Length > 50)
                throw new BusinessException("Name cannot have more than 50 characters.");
            if (!string.IsNullOrEmpty(description) && description.Length > 160)
                throw new BusinessException("Description cannot have more than 160 characters.");

            byte[] picture = null;
            if (changePicture && pictureStream != null)
                picture = GetPictureBytes(pictureStream, pictureExtension);

            var advisor = Data.GetAdvisor(id);
            if (advisor == null || !advisor.Enabled)
                throw new NotFoundException("Trader not found");
            if (advisor.Email.ToLower() != LoggedEmail.ToLower())
                throw new UnauthorizedException("Invalid credentials");

            var previousData = $"(Previous) Name: {advisor.Name} - Change Picture: {changePicture} - Url Guid: {advisor.UrlGuid} - Description: {advisor.Description}";

            if (changePicture)
            {
                var previousGuid = advisor.UrlGuid;
                advisor.UrlGuid = Guid.NewGuid();
                if (await AzureStorageBusiness.UploadUserPictureFromBytesAsync($"{advisor.UrlGuid}.png", picture ?? GetNoUploadedImageForAdvisor(advisor)))
                    await AzureStorageBusiness.DeleteUserPicture($"{previousGuid}.png");
            }
            advisor.Name = name;
            advisor.Description = description;
            Update(advisor);

            ActionBusiness.InsertEditAdvisor(advisor.Id, previousData);
            var cachedAdvisor = AdvisorRankingBusiness.ListAdvisorsFullData().FirstOrDefault(c => c.Id == advisor.Id);
            if (cachedAdvisor != null)
            {
                cachedAdvisor.Name = advisor.Name;
                cachedAdvisor.Description = advisor.Description;
                cachedAdvisor.UrlGuid = advisor.UrlGuid;
            }

            return advisor.UrlGuid;
        }

        public byte[] GetNoUploadedImageForAdvisor(User user)
        {
            var userData = (user.CreationDate.Ticks * (double)user.Id + user.Id).ToString("##############################################################").Select(c => Convert.ToInt32(c.ToString()));
            var start = userData.Sum() % 6;
            var colorDistribution = new Dictionary<int, Color>();
            colorDistribution[start] = ColorTranslator.FromHtml("#ffffff");
            colorDistribution[(start + 1) % 6] = ColorTranslator.FromHtml("#07d1ff");
            colorDistribution[(start + 2) % 6] = ColorTranslator.FromHtml("#343434");
            colorDistribution[(start + 3) % 6] = ColorTranslator.FromHtml("#50e3c2");
            colorDistribution[(start + 4) % 6] = ColorTranslator.FromHtml("#126efd");
            colorDistribution[(start + 5) % 6] = ColorTranslator.FromHtml("#ce9355");
            using (var bitmap = new Bitmap(32, 32))
            {
                var dataPosition = 0;
                for (var w = 0; w < 32; ++w)
                {
                    for (var h = 0; h < 32; ++h)
                    {
                        if (userData.ElementAt(dataPosition) < 3)
                            bitmap.SetPixel(w, h, colorDistribution[0]);
                        else if (userData.ElementAt(dataPosition) < 5)
                            bitmap.SetPixel(w, h, colorDistribution[1]);
                        else if (userData.ElementAt(dataPosition) == 5)
                            bitmap.SetPixel(w, h, colorDistribution[2]);
                        else if (userData.ElementAt(dataPosition) == 6)
                            bitmap.SetPixel(w, h, colorDistribution[3]);
                        else if (userData.ElementAt(dataPosition) < 9)
                            bitmap.SetPixel(w, h, colorDistribution[4]);
                        else
                            bitmap.SetPixel(w, h, colorDistribution[5]);

                        if (dataPosition == userData.Count() - 1)
                            dataPosition = 0;
                        else
                            ++dataPosition;
                    }
                }
                using (var stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Png);
                    stream.Position = 0;
                    return stream.ToArray();
                }
            }
        }

        public byte[] GetPictureBytes(Stream pictureStream, string pictureExtension)
        {
            pictureExtension = pictureExtension == "JPEG" ? "JPG" : pictureExtension;
            var extensionFound = FileTypeMatcher.GetFileExtension(pictureStream);
            if (string.IsNullOrEmpty(extensionFound) || pictureExtension != extensionFound)
                throw new BusinessException("File is invalid.");

            byte[] picture;
            using (var memoryStream = new MemoryStream())
            {
                pictureStream.CopyTo(memoryStream);
                picture = memoryStream.ToArray();
            }
            if (picture.Length > (1 * 1024 * 1024))
                throw new BusinessException("File is too big. The maximum is 1MB.");

            return picture;
        }

        public DomainObjects.Advisor.Advisor GetAdvisor(int id)
        {
            return Data.GetAdvisor(id);
        }

        public List<DomainObjects.Advisor.Advisor> ListAllAdvisors()
        {
            return Data.ListEnabled();
        }

        public IEnumerable<AdvisorResponse> ListAdvisorsData()
        {
            var advisors = AdvisorRankingBusiness.ListAdvisorsFullData();
            var advisorsFollowers = FollowAdvisorBusiness.ListFollowers(advisors.Select(c => c.Id).Distinct(), false);
            var user = GetLoggedUser();
            return advisors.Select(c => AdvisorRankingBusiness.GetAdvisorResponse(c, advisors.Count, advisorsFollowers, user, null, null, null, null)).OrderBy(c => c.Ranking);
        }

        public IEnumerable<AdvisorResponse> ListAdvisorsMonthlyRanking(int? year, int? month)
        {
            var advisors = AdvisorRankingBusiness.ListAdvisorsFullData();
            var advisorsIds = advisors.Select(c => c.Id).Distinct().ToHashSet();
            var advisorsFollowers = FollowAdvisorBusiness.ListFollowers(advisorsIds, false);
            var user = GetLoggedUser();

            List<AdvisorMonthlyRanking> ranking = null;
            List<AdvisorRankingHistory> monthBeginningHistory = null;
            if (year.HasValue && month.HasValue && !(year == Data.GetDateTimeNow().Year && month == Data.GetDateTimeNow().Month))
                ranking = AdvisorMonthlyRankingBusiness.ListAdvisorsMonthlyRanking(year.Value, month.Value);
            if (ranking == null || ranking.Count == 0)
                monthBeginningHistory = AdvisorRankingHistoryBusiness.ListAdvisorsRankingAndProfitForMonthBeginning(advisorsIds);

            var result = new List<AdvisorResponse>();
            foreach(var advisor in advisors)
            {
                if (monthBeginningHistory != null)
                {
                    var data = monthBeginningHistory.FirstOrDefault(c => c.UserId == advisor.Id);
                    if (data != null)
                        result.Add(AdvisorRankingBusiness.GetAdvisorResponse(advisor, advisors.Count, advisorsFollowers, user, null, null, data, null));
                }
                else
                {
                    var data = ranking.FirstOrDefault(c => c.UserId == advisor.Id);
                    if (data != null)
                        result.Add(AdvisorRankingBusiness.GetAdvisorResponse(advisor, advisors.Count, advisorsFollowers, user, null, null, null, data));
                }
            }
            if (monthBeginningHistory != null)
            {
                result = result.OrderByDescending(c => c.MonthlyRankingHistory.ProfitPercentage).ThenByDescending(c => c.UserId).ToList();
                for (int i = 0; i < result.Count; ++i)
                    result[i].MonthlyRankingHistory.Ranking = i + 1;
            }
            else
                result = result.OrderBy(c => c.MonthlyRankingHistory.Ranking).ToList();

            return result;
        }

        public AdvisorResponse GetAdvisorData(int advisorId)
        {
            AdvisorRanking advisor = null;
            List<DomainObjects.Asset.Asset> assets = null;
            List<FollowAdvisor> advisorsFollowers = null;
            AdvisorRankingHistory advisorHistory = null;
            Parallel.Invoke(() => advisor = AdvisorRankingBusiness.GetAdvisorFullData(advisorId),
                            () => assets = AssetBusiness.ListAssets(false),
                            () => advisorsFollowers = FollowAdvisorBusiness.ListFollowers(new int[] { advisorId }, false),
                            () => advisorHistory = AdvisorRankingHistoryBusiness.GetLastAdvisorRankingAndProfit(advisorId));

            return AdvisorRankingBusiness.GetAdvisorResponse(advisor, advisor.TotalAdvisors, advisorsFollowers, GetLoggedUser(), assets, advisorHistory, null, null);
        }

        public AdvisorPerformanceResponse GetAdvisorPeformance(int advisorId)
        {
            AdvisorRanking advisor = AdvisorRankingBusiness.GetAdvisorFullData(advisorId);
            if (advisor == null)
                throw new NotFoundException("Advisor not found.");
            
            List<DailyPerformanceResponse> dailyPerformance = null;
            List<Order> closedOrders = null;
            Parallel.Invoke(() => dailyPerformance = AdvisorRankingHistoryBusiness.ListDailyPerformance(advisorId),
                            () => closedOrders = OrderBusiness.ListOrders(new int[] { advisorId }, null, new OrderStatusType[] { OrderStatusType.Close }));

            return new AdvisorPerformanceResponse()
            {
                DailyPerformance = dailyPerformance,
                BestTrade = closedOrders.Any() ? closedOrders.Max(c => c.Profit ?? double.MinValue) : 0,
                WorstTrade = closedOrders.Any() ? closedOrders.Min(c => c.Profit ?? double.MinValue) : 0,
                DailyDrawdown = dailyPerformance.Any() ? dailyPerformance.Min(c => c.Variation) : 0
            };
        }

        public AdvisorResponse GetLoggedAdvisor()
        {
            var loggedUser = GetValidUser();
            return GetAdvisorData(loggedUser.Id);
        }
        
        public IEnumerable<AdvisorResponse> ListAdvisorsFollowedByUser(int userId)
        {
            var allAdvisors = ListAdvisorsData();
            var advisorsFollowed = Data.ListFollowingAdvisors(userId);

            return allAdvisors.Where(advisor => advisorsFollowed.Select(followed => followed.Id).Contains(advisor.UserId));
        }

        public IEnumerable<DomainObjects.Advisor.Advisor> ListByName(string searchTerm)
        {
            return AdvisorRankingBusiness.ListAdvisorsFullData().Where(advisor => advisor.Name.ToUpper().StartsWith(searchTerm.ToUpper()) || advisor.Name.ToUpper().Contains(" " + searchTerm.ToUpper()));
        }
    }
}
