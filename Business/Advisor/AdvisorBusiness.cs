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

namespace Auctus.Business.Advisor
{
    public class AdvisorBusiness : BaseBusiness<DomainObjects.Advisor.Advisor, IAdvisorData<DomainObjects.Advisor.Advisor>>
    {
        public enum CalculationMode { AdvisorBase = 0, AdvisorDetailed = 1, AssetBase = 2, AssetDetailed = 3, Feed = 4, AssetRatings = 5 }
        private const string ADVISORS_CACHE_KEY = "Advisors";

        public AdvisorBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public async Task<LoginResponse> CreateAsync(string email, string password, string name, string description,
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
            if (LoggedEmail != null)
            {
                if (!string.IsNullOrWhiteSpace(email) && email != LoggedEmail)
                    throw new BusinessException("Invalid email.");
                if (!string.IsNullOrWhiteSpace(password))
                    throw new BusinessException("Invalid password.");

                user = UserBusiness.GetForLoginByEmail(LoggedEmail);
                if (UserBusiness.IsValidAdvisor(user))
                    throw new BusinessException("User already registered.");
            }
            else
                user = UserBusiness.GetValidUserToRegister(email, password, null);

            Guid urlGuid = Guid.NewGuid();
            await AzureStorageBusiness.UploadUserPictureFromBytesAsync($"{urlGuid}.png", picture ?? AdvisorBusiness.GetNoUploadedImageForAdvisor(user));
          
            using (var transaction = TransactionalDapperCommand)
            {
                if (LoggedEmail == null)
                    transaction.Insert(user);

                var advisor = new DomainObjects.Advisor.Advisor()
                {
                    Id = user.Id,
                    Name = name,
                    Description = description,
                    BecameAdvisorDate = Data.GetDateTimeNow(),
                    Enabled = true,
                    UrlGuid = urlGuid
                };
                transaction.Insert(advisor);
                transaction.Commit();
            }

            await UserBusiness.SendEmailConfirmationAsync(user.Email, user.ConfirmationCode);

            return new LoginResponse()
            {
                Id = user.Id,
                Email = user.Email,
                PendingConfirmation = !user.ConfirmationDate.HasValue,
                AdvisorName = name,
                ProfileUrlGuid = urlGuid.ToString(),
                HasInvestment = true,
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
            if (string.IsNullOrWhiteSpace(description))
                throw new BusinessException("Description must be filled.");
            if (description.Length > 160)
                throw new BusinessException("Description cannot have more than 160 characters.");

            byte[] picture = null;
            if (changePicture && pictureStream != null)
                picture = GetPictureBytes(pictureStream, pictureExtension);

            var advisor = Data.GetAdvisor(id);
            if (advisor == null || !advisor.Enabled)
                throw new NotFoundException("Expert not found");
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
            UpdateAdvisorsCacheAsync();

            return advisor.UrlGuid;
        }

        public void UpdateAdvisorsCacheAsync()
        {
            RunAsync(() => UpdateAdvisorsCache(Data.ListEnabled()));
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

        public IEnumerable<AdvisorResponse> ListAdvisorsData()
        {
            List<AdvisorResponse> advisorsResult;
            List<AssetResponse> assetsResult;
            var user = GetLoggedUser();
            CalculateForAdvisorsData(user, CalculationMode.AdvisorBase, out advisorsResult, out assetsResult);
            return advisorsResult;
        }

        public AdvisorResponse GetAdvisorData(int advisorId)
        {
            List<AdvisorResponse> advisorsResult;
            List<AssetResponse> assetsResult;
            var user = GetLoggedUser();
            CalculateForAdvisorsData(user, CalculationMode.AdvisorDetailed, out advisorsResult, out assetsResult, advisorId);
            var result = advisorsResult.Single(c => c.UserId == advisorId);
            result.Assets = assetsResult.Where(c => c.AssetAdvisor.Any(a => a.UserId == advisorId)).ToList();
            result.Assets.ForEach(a => a.AssetAdvisor = a.AssetAdvisor.Where(c => c.UserId == advisorId).ToList());
            result.Assets = result.Assets.OrderByDescending(a => a.AssetAdvisor.FirstOrDefault().LastAdviceDate).ToList();
            return result;
        }

        public DomainObjects.Advisor.Advisor CreateFromRequest(RequestToBeAdvisor request, Guid urlGuid)
        {
            var advisor = new DomainObjects.Advisor.Advisor()
            {
                Id = request.UserId,
                Name = request.Name,
                Description = request.Description,
                BecameAdvisorDate = Data.GetDateTimeNow(),
                Enabled = true,
                UrlGuid = urlGuid
            };
            return advisor;
        }

        private void CalculateForAdvisorsData(User user, CalculationMode mode, out List<AdvisorResponse> advisorsResult, out List<AssetResponse> assetsResult, int? advisorId = null)
        {
            var advisors = GetAdvisors();
            var advices = Task.Factory.StartNew(() => AdviceBusiness.List(advisors.Select(c => c.Id).Distinct()));
            var advisorFollowers = Task.Factory.StartNew(() => FollowAdvisorBusiness.ListFollowers(advisors.Select(c => c.Id).Distinct()));
            var assetFollowers = Task.Factory.StartNew(() => FollowAssetBusiness.ListFollowers());
            Task.WaitAll(advices, advisorFollowers, assetFollowers);

            Calculation(mode, out advisorsResult, out assetsResult, user, advices.Result, advisors, advisorFollowers.Result, assetFollowers.Result, null, advisorId);
        }

        public List<DomainObjects.Advisor.Advisor> GetAdvisors()
        {
            var advisors = MemoryCache.Get<List<DomainObjects.Advisor.Advisor>>(ADVISORS_CACHE_KEY);
            if (advisors == null)
            {
                advisors = Data.ListEnabled();
                UpdateAdvisorsCache(advisors);
            }
            return advisors;
        }

        private void UpdateAdvisorsCache(List<DomainObjects.Advisor.Advisor> advisors)
        {
            if (advisors != null && advisors.Any())
                MemoryCache.Set(ADVISORS_CACHE_KEY, advisors, 120);
        }

        public void Calculation(CalculationMode mode, out List<AdvisorResponse> advisorsResult, out List<AssetResponse> assetsResult, User loggedUser, 
            IEnumerable<Advice> allAdvices, IEnumerable<DomainObjects.Advisor.Advisor> allAdvisors, IEnumerable<FollowAdvisor> advisorFollowers,
            IEnumerable<FollowAsset> assetFollowers = null, IEnumerable<int> selectAssetsId = null, int? selectAdvisorId = null)
        {
            advisorsResult = new List<AdvisorResponse>();
            assetsResult = new List<AssetResponse>();

            var assetsIds = allAdvices?.Select(a => a.AssetId);
            if (assetsIds?.Any() == true)
            {
                assetsIds = assetsIds.Distinct().ToHashSet();

                var selectAssetId = selectAssetsId != null && selectAssetsId.Count() == 1 ? (int?)selectAssetsId.First() : null;
                if (selectAssetId.HasValue && !assetsIds.Contains(selectAssetId.Value))
                {
                    FillNotAdvicedAsset(out assetsResult, mode, selectAssetsId, loggedUser, assetFollowers);
                    return;
                }

                var assets = AssetCurrentValueBusiness.ListAssetsValuesForCalculation(assetsIds, mode, allAdvices, selectAssetId, selectAdvisorId);

                var adviceDetails = new List<AdviceDetail>();
                foreach (var asset in assets)
                {
                    var assetAdviceDetails = new List<AdviceDetail>();
                    var previousAdvice = new Dictionary<int, AdviceDetail>();
                    var startAdviceType = new Dictionary<int, AdviceDetail>();
                    var assetAdvices = allAdvices.Where(a => a.AssetId == asset.Id).OrderBy(c => c.CreationDate);
                    foreach (var advice in assetAdvices)
                    {
                        var detail = SetAdviceDetail(assetAdviceDetails, advice, previousAdvice.ContainsKey(advice.AdvisorId) ? previousAdvice[advice.AdvisorId] : null,
                            startAdviceType.ContainsKey(advice.AdvisorId) ? startAdviceType[advice.AdvisorId] : null);
                        if (detail != null)
                        {
                            previousAdvice[advice.AdvisorId] = detail;
                            if (detail.Advice.AdviceType == AdviceType.ClosePosition)
                                startAdviceType[advice.AdvisorId] = null;
                            else if (!startAdviceType.ContainsKey(advice.AdvisorId) || startAdviceType[advice.AdvisorId] == null || 
                                startAdviceType[advice.AdvisorId].Advice.Type != detail.Advice.Type)
                                startAdviceType[advice.AdvisorId] = detail;
                        }   
                    }

                    var assetAdvisorsId = assetAdvices.Select(c => c.AdvisorId).Distinct().ToHashSet();

                    AssetResponse assetResultData = null;
                    if (mode != CalculationMode.AdvisorBase)
                        assetResultData = GetAssetBaseResponse(loggedUser, asset, assetFollowers, assetAdvices, assetAdvisorsId, mode);

                    foreach (var advisorId in assetAdvisorsId)
                    {
                        SetAdviceDetail(assetAdviceDetails, GetLastAdvice(asset, advisorId, asset.CurrentValue), 
                            previousAdvice.ContainsKey(advisorId) ? previousAdvice[advisorId] : null, startAdviceType.ContainsKey(advisorId) ? startAdviceType[advisorId] : null);

                        if (mode != CalculationMode.AdvisorBase)
                            assetResultData.AssetAdvisor.Add(GetAssetAdvisorResponse(advisorId, assetAdviceDetails, mode));
                    }

                    if (mode != CalculationMode.AdvisorBase)
                    {
                        if (mode != CalculationMode.AdvisorDetailed && mode != CalculationMode.Feed)
                        {
                            assetResultData.RecommendationDistribution = assetResultData.AssetAdvisor.Where(c => c.LastAdviceType.HasValue).GroupBy(c => c.LastAdviceType.Value)
                                .Select(g => new RecommendationDistributionResponse() { Type = g.Key, Total = g.Count() }).ToList();
                            assetResultData.Mode = GetAssetModeType(assetResultData);
                            assetResultData.Advices = mode == CalculationMode.AssetBase ? null : assetAdviceDetails
                                .Select(c => 
                                new AssetResponse.AdviceResponse()
                                {
                                    UserId = c.Advice.AdvisorId,
                                    AdviceType = c.Advice.Type,
                                    Date = c.Advice.CreationDate,
                                    AssetValue = c.Advice.AssetValue,
                                    OperationType = c.Advice.OperationType,
                                    TargetPrice = c.Advice.TargetPrice,
                                    StopLoss = c.Advice.StopLoss
                                }).OrderBy(c => c.Date).ToList();
                        }
                        assetsResult.Add(assetResultData);
                    }

                    adviceDetails.AddRange(assetAdviceDetails);
                }
                var advisorsData = new Dictionary<int, IEnumerable<AdviceDetail>>();
                if (mode != CalculationMode.AssetBase && allAdvisors?.Any() == true)
                {
                    foreach (var advisor in allAdvisors)
                    {
                        var details = adviceDetails.Where(c => c.Advice.AdvisorId == advisor.Id);
                        advisorsResult.Add(GetAdvisorResponse(details, advisorFollowers, advisor, loggedUser));
                        advisorsData[advisor.Id] = details;
                    }
                    SetAdvisorsRanking(ref advisorsResult, advisorsData);
                }
            }
            else if (selectAssetsId?.Any() == true)
                FillNotAdvicedAsset(out assetsResult, mode, selectAssetsId, loggedUser, assetFollowers);
        }

        private void FillNotAdvicedAsset(out List<AssetResponse> assetsResult, CalculationMode mode, IEnumerable<int> selectAssetsId, User loggedUser, IEnumerable<FollowAsset> assetFollowers)
        {
            assetsResult = new List<AssetResponse>();
            var assets = AssetCurrentValueBusiness.ListAssetsValuesForCalculation(selectAssetsId, mode, null, selectAssetsId.Count() == 1 ? (int?)selectAssetsId.First() : null);
            foreach(var asset in assets)
                assetsResult.Add(GetAssetBaseResponse(loggedUser, asset, assetFollowers, new Advice[] { }, new int[] { }, mode));
        }

        private Advice GetLastAdvice(DomainObjects.Asset.Asset asset, int advisorId, double lastValue)
        {
            return new Advice()
            {
                AssetId = asset.Id,
                CreationDate = Data.GetDateTimeNow(),
                Type = AdviceType.ClosePosition.Value,
                AdvisorId = advisorId,
                AssetValue = lastValue
            };
        }

        private AssetResponse GetAssetBaseResponse(User loggedUser, AssetCurrentValue assetCurrentValue, IEnumerable<FollowAsset> assetFollowers, 
            IEnumerable<Advice> assetAdvices, IEnumerable<int> assetAdvisorsId, CalculationMode mode)
        {
            var assFollowers = assetFollowers?.Where(c => c.AssetId == assetCurrentValue.Id);
            return new AssetResponse()
            {
                AssetId = assetCurrentValue.Id,
                Code = assetCurrentValue.Code,
                Name = assetCurrentValue.Name,
                Following = loggedUser != null && assFollowers?.Any(c => c.UserId == loggedUser.Id) == true,
                NumberOfFollowers = assFollowers?.Count(),
                TotalAdvisors = assetAdvisorsId.Count(),
                TotalRatings = assetAdvices.Count(),
                LastValue = assetCurrentValue.CurrentValue,
                MarketCap = assetCurrentValue.MarketCap,
                Variation24h = assetCurrentValue.Variation24Hours,
                Variation7d = assetCurrentValue.Variation7Days,
                Variation30d = assetCurrentValue.Variation30Days
            };
        }

        private AssetResponse.AssetAdvisorResponse GetAssetAdvisorResponse(int advisorId, List<AdviceDetail> assetAdviceDetails, CalculationMode mode)
        {
            var advisorDetailsValues = assetAdviceDetails.Where(c => c.Advice.AdvisorId == advisorId).OrderBy(c => c.Advice.CreationDate);
            var lastAdvice = advisorDetailsValues.LastOrDefault();
            return new AssetResponse.AssetAdvisorResponse()
            {
                UserId = advisorId,
                AverageReturn = advisorDetailsValues.Where(c => c.Return.HasValue).Sum(c => c.Return.Value) / advisorDetailsValues.Count(c => c.Return.HasValue),
                SuccessRate = (double)advisorDetailsValues.Count(c => c.Success.HasValue && c.Success.Value) / advisorDetailsValues.Count(c => c.Success.HasValue),
                CurrentReturn = lastAdvice == null || lastAdvice.Advice.AdviceType == AdviceType.ClosePosition ? 0 : lastAdvice.Return ?? 0,
                TotalRatings = advisorDetailsValues.Count(),
                LastAdviceDate = lastAdvice?.Advice.CreationDate,
                LastAdviceMode = lastAdvice?.ModeType.Value,
                LastAdviceType = lastAdvice?.Advice.Type,
                LastAdviceAssetValue = lastAdvice?.Advice.AssetValue,
                LastAdviceOperationType = lastAdvice?.Advice.OperationType,
                LastAdviceTargetPrice = lastAdvice?.Advice.TargetPrice,
                LastAdviceStopLoss = lastAdvice?.Advice.StopLoss,
                Advices = mode == CalculationMode.AdvisorDetailed ? advisorDetailsValues.Select(c =>
                    new AssetResponse.AdviceResponse()
                    {
                        UserId = advisorId,
                        AdviceType = c.Advice.Type,
                        Date = c.Advice.CreationDate,
                        AssetValue = c.Advice.AssetValue,
                        OperationType = c.Advice.OperationType,
                        TargetPrice = c.Advice.TargetPrice,
                        StopLoss = c.Advice.StopLoss
                    }).ToList() : null
            };
        }

        private AdvisorResponse GetAdvisorResponse(IEnumerable<AdviceDetail> details, IEnumerable<FollowAdvisor> advisorFollowers, DomainObjects.Advisor.Advisor advisor, User loggedUser)
        {
            var assetsAdvised = details.Select(c => c.Advice.AssetId);
            var advFollowers = advisorFollowers?.Where(c => c.AdvisorId == advisor.Id);
            return new AdvisorResponse()
            {
                UserId = advisor.Id,
                Name = advisor.Name,
                UrlGuid = advisor.UrlGuid.ToString(),
                CreationDate = advisor.BecameAdvisorDate,
                Description = advisor.Description,
                Owner = loggedUser != null && advisor.Id == loggedUser.Id,
                NumberOfFollowers = advFollowers?.Count() ?? 0,
                TotalAssetsAdvised = assetsAdvised.Any() ? assetsAdvised.Distinct().Count() : 0,
                Following = loggedUser != null && advFollowers?.Any(c => c.UserId == loggedUser.Id) == true,
                AverageReturn = details.Any(c => c.Return.HasValue) ? details.Where(c => c.Return.HasValue).Sum(c => c.Return.Value) / details.Count(c => c.Return.HasValue) : 0,
                SuccessRate = details.Any(c => c.Success.HasValue) ? (double)details.Count(c => c.Success.HasValue && c.Success.Value) / details.Count(c => c.Success.HasValue) : 0,
                RecommendationDistribution = !details.Any() ? new List<RecommendationDistributionResponse>() :
                    details.GroupBy(c => c.Advice.Type).Select(g => new RecommendationDistributionResponse() { Type = g.Key, Total = g.Count() }).ToList()
            };
        }

        private void SetAdvisorsRanking(ref List<AdvisorResponse> advisorsResult, Dictionary<int, IEnumerable<AdviceDetail>> advisorsData)
        {
            var advisorsConsidered = advisorsResult.Where(c => c.CreationDate < Data.GetDateTimeNow().AddDays(-3));
            if (!advisorsConsidered.Any())
                advisorsConsidered = advisorsResult;

            var newAdvisors = advisorsResult.Where(c => !advisorsConsidered.Any(a => a.UserId == c.UserId));
            var details = advisorsData.Where(c => advisorsConsidered.Any(a => a.UserId == c.Key));

            var maxAvg = advisorsConsidered.Max(c => c.AverageReturn);
            var maxSucRate = advisorsConsidered.Max(c => c.SuccessRate);
            var maxAssets = advisorsConsidered.Max(c => c.TotalAssetsAdvised);
            var lastActivity = details.Any(c => c.Value.Any()) ? details.Where(c => c.Value.Any()).Max(c => c.Value.Max(a => a.Advice.CreationDate)) : DateTime.MinValue;

            var advDays = advisorsResult.Select(c => new { Id = c.UserId, Days = Data.GetDateTimeNow().Subtract(c.CreationDate).TotalDays }).ToDictionary(c => c.Id, c => c.Days);
            var maxAdvices = details.Any(c => c.Value.Any()) ? details.Max(c => (double)c.Value.Count() / advDays[c.Key]) : 0;
            var maxFollowers = advisorsConsidered.Max(c => (double)c.NumberOfFollowers / advDays[c.UserId]);

            var generalNormalization = 1.2;
            advisorsResult.ForEach(c =>
            {
                var maximumValue = newAdvisors.Any(a => a.UserId == c.UserId) ? 0.7 : 1.0;
                var rating = Math.Min(5.0, generalNormalization * (
                      (0.35 * 5.0 * Math.Min(maximumValue, maxAvg <= 0 || c.AverageReturn <= 0 ? 0 : c.AverageReturn / maxAvg))
                    + (0.30 * 5.0 * Math.Min(maximumValue, maxSucRate == 0 ? 0 : c.SuccessRate / maxSucRate))
                    + (0.01 * 5.0 * Math.Min(maximumValue, maxAssets == 0 ? 0 : (double)c.TotalAssetsAdvised / maxAssets))
                    + (0.15 * 5.0 * Math.Min(maximumValue, maxAdvices == 0 ? 0 : ((double)advisorsData[c.UserId].Count() / advDays[c.UserId]) / maxAdvices))
                    + (0.15 * 5.0 * Math.Min(maximumValue, maxFollowers == 0 ? 0 : ((double)c.NumberOfFollowers / advDays[c.UserId]) / maxFollowers))
                    + (0.04 * 5.0 * Math.Min(maximumValue, lastActivity.Ticks == 0 ? 0 : (double)c.CreationDate.Ticks / lastActivity.Ticks))));
                c.Rating = 2.5 + (2.5 * rating / 5.0);
            });
            advisorsResult = advisorsResult.OrderByDescending(c => c.Rating).ToList();
            for (int i = 0; i < advisorsResult.Count; ++i)
            {
                advisorsResult[i].Ranking = i + 1;
                advisorsResult[i].TotalAdvisors = advisorsResult.Count;
            }
        }

        private AdviceDetail SetAdviceDetail(List<AdviceDetail> assetAdviceDetails, Advice advice, AdviceDetail previousAdvice, AdviceDetail startAdviceType)
        {
            if (previousAdvice != null)
            {
                if (previousAdvice.Advice.Type != advice.Type)
                    previousAdvice.Return = previousAdvice.Advice.AdviceType == AdviceType.ClosePosition ? (double?)null :
                                        (previousAdvice.Advice.AdviceType == AdviceType.Buy ? 1.0 : -1.0) * (advice.AssetValue / previousAdvice.Advice.AssetValue - 1);

                assetAdviceDetails.Add(previousAdvice);
            }
            if (startAdviceType != null && startAdviceType.Advice.Type != advice.Type)
            {
                var advisorAdvices = assetAdviceDetails.Where(c => c.Advice.AdvisorId == startAdviceType.Advice.AdvisorId && startAdviceType.Advice.Id <= c.Advice.Id
                                        && startAdviceType.Advice.Type == c.Advice.Type).ToList();
                advisorAdvices.ForEach(c =>
                {
                    if (c.Advice.AdviceType != AdviceType.ClosePosition)
                    {
                        c.Return = (startAdviceType.Advice.AdviceType == AdviceType.Buy ? 1.0 : -1.0) * (advice.AssetValue / c.Advice.AssetValue - 1);
                        c.Success = startAdviceType.Advice.AdviceType == AdviceType.Buy ? advice.AssetValue >= c.Advice.AssetValue : advice.AssetValue <= c.Advice.AssetValue;
                    }
                });
            }
            return new AdviceDetail()
            {
                Advice = advice,
                ModeType = previousAdvice == null ? AdviceModeType.Initiate : 
                            previousAdvice.Advice.Type == advice.Type ? AdviceModeType.Reiterate :
                            previousAdvice.Advice.AdviceType == AdviceType.ClosePosition ? AdviceModeType.Initiate :
                            previousAdvice.Advice.AdviceType == AdviceType.Buy ? AdviceModeType.Downgrade : AdviceModeType.Upgrade
            };
        }

        private int GetAssetModeType(AssetResponse asset)
        {
            if (asset.RecommendationDistribution.Any())
            {
                var total = asset.RecommendationDistribution.Sum(c => c.Total);
                var percentages = asset.RecommendationDistribution.Select(c => new { Type = c.Type, Percentage = 100.0 * c.Total / total });
                var majorityType = percentages.SingleOrDefault(c => c.Percentage > 50);
                if (majorityType != null && majorityType.Type != AdviceType.ClosePosition.Value)
                {
                    if (total < 5 || majorityType.Percentage < 70)
                        return majorityType.Type == AdviceType.Buy.Value ? AssetModeType.ModerateBuy.Value : AssetModeType.ModerateSell.Value;
                    else
                        return majorityType.Type == AdviceType.Buy.Value ? AssetModeType.StrongBuy.Value : AssetModeType.StrongSell.Value;
                }
            }
            return AssetModeType.Neutral.Value;
        }

        private class AdviceDetail
        {
            public Advice Advice{ get; set; }
            public double? Return { get; set; }
            public bool? Success { get; set; }
            public AdviceModeType ModeType { get; set; }
        }

        public void Advise(int assetId, AdviceType type, double? stopLoss, double? targetPrice)
        {
            var user = GetValidUser();
            if (!UserBusiness.IsValidAdvisor(user))
                throw new UnauthorizedException("Logged user is not a valid Expert.");

            var asset = AssetBusiness.GetById(assetId);
            if (asset == null)
                throw new NotFoundException("Asset not found.");

            AdviceBusiness.ValidateAndCreate((DomainObjects.Advisor.Advisor)user, asset, type, stopLoss, targetPrice);
        }

        public IEnumerable<DomainObjects.Advisor.Advisor> ListFollowingAdvisors()
        {
            var user = GetValidUser();
            return Data.ListFollowingAdvisors(user.Id);
        }

        public IEnumerable<DomainObjects.Advisor.Advisor> ListByName(string searchTerm)
        {
            return GetAdvisors().Where(advisor => advisor.Name.ToUpper().StartsWith(searchTerm.ToUpper()) || advisor.Name.ToUpper().Contains(" " + searchTerm.ToUpper()));
        }
    }
}
