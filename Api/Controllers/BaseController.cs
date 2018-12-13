using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Auctus.Util;
using Api.Model;
using Microsoft.Extensions.Primitives;
using Auctus.Business.Account;
using Auctus.Business.Advisor;
using Auctus.Business.Asset;
using Microsoft.Extensions.Configuration;
using Microsoft.ApplicationInsights;
using System.Net;
using Auctus.Util.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Auctus.Business.Event;
using Auctus.Business.Storage;
using Auctus.Business.News;
using Microsoft.AspNetCore.SignalR;
using Api.Hubs;
using Auctus.Business.Exchange;
using Auctus.Business.Trade;
using Auctus.Business.Email;

namespace Api.Controllers
{
    public class BaseController : Controller
    {
        protected ILoggerFactory LoggerFactory { get; private set; }
        protected ILogger Logger { get; private set; }
        protected Cache MemoryCache { get; private set; }
        protected IServiceProvider ServiceProvider { get; private set; }
        protected IServiceScopeFactory ServiceScopeFactory { get; private set; }
        protected readonly IHubContext<AuctusHub> HubContext;

        protected BaseController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, IHubContext<AuctusHub> hubContext)
        {
            MemoryCache = cache;
            LoggerFactory = loggerFactory;
            Logger = loggerFactory.CreateLogger(GetType().Namespace);
            ServiceProvider = serviceProvider;
            ServiceScopeFactory = serviceScopeFactory;
            HubContext = hubContext;
        }

        protected string GetUser()
        {
            return Request.HttpContext.User.Identity.IsAuthenticated ? Request.HttpContext.User.Identity.Name : null;
        }

        protected string GetRequestIP(bool tryUseXForwardHeader = true)
        {
            string ip = null;
   
            if (tryUseXForwardHeader)
                ip = SplitCsv(GetHeaderValueAs<string>("X-Forwarded-For")).FirstOrDefault();
            
            if (string.IsNullOrWhiteSpace(ip) && Request?.HttpContext?.Connection?.RemoteIpAddress != null)
                ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();

            if (string.IsNullOrWhiteSpace(ip))
                ip = GetHeaderValueAs<string>("REMOTE_ADDR");

            return ip;
        }

        protected T GetHeaderValueAs<T>(string headerName)
        {
            StringValues values;

            if (Request?.Headers?.TryGetValue(headerName, out values) == true)
            {
                string rawValues = values.ToString();
                if (!string.IsNullOrEmpty(rawValues))
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }

            return default(T);
        }

        private List<string> SplitCsv(string csvList, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(csvList))
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

            return csvList
                .TrimEnd(',')
                .Split(',')
                .AsEnumerable<string>()
                .Select(s => s.Trim())
                .ToList();
        }

        protected bool RequestHasFile()
        {
            return Request.Form != null && Request.Form.Files != null && Request.Form.Files.Count == 1 && Request.Form.Files[0].Length > 0
                && !string.IsNullOrWhiteSpace(Request.Form.Files[0].FileName) && !string.IsNullOrWhiteSpace(Request.Form.Files[0].ContentType);
        }

        protected string GetValidPictureExtension()
        {
            if (!RequestHasFile())
                throw new BusinessException("File not found.");

            if (!FileTypeMatcher.GetValidFileExtensions().Any(c => Request.Form.Files[0].ContentType.ToUpper().Contains(c)))
                throw new BusinessException("Invalid file.");

            var fileExtension = Request.Form.Files[0].FileName.Split('.').Last().ToUpper();
            if (string.IsNullOrWhiteSpace(fileExtension) || !FileTypeMatcher.GetValidFileExtensions().Any(c => c == fileExtension))
                throw new BusinessException("File extension is invalid.");

            return fileExtension;
        }

        protected new OkObjectResult Ok()
        {
            return Ok(new { });
        }

        protected new BadRequestObjectResult BadRequest()
        {
            return BadRequest(new { });
        }

        protected void RunAsync(Action action)
        {
            Task.Factory.StartNew(() =>
            {
                using (var scope = ServiceScopeFactory.CreateScope())
                {
                    ServiceProvider = scope.ServiceProvider;
                    TelemetryClient telemetry = new TelemetryClient();
                    try
                    {
                        telemetry.TrackEvent($"Job {action.Method.Name} started ({DateTime.UtcNow.ToLongDateString()}).");
                        action();
                        telemetry.TrackEvent($"Job {action.Method.Name} ended ({DateTime.UtcNow.ToLongDateString()}).");
                    }
                    catch (Exception e)
                    {
                        telemetry.TrackException(e);
                    }
                    finally
                    {
                        telemetry.Flush();
                    }
                }
            });
        }

        protected void RunAsync(Func<Task> action)
        {
            Task.Factory.StartNew(() =>
            {
                using (var scope = ServiceScopeFactory.CreateScope())
                {
                    ServiceProvider = scope.ServiceProvider;
                    TelemetryClient telemetry = new TelemetryClient();
                    try
                    {
                        telemetry.TrackEvent($"Job {action.Method.Name} started ({DateTime.UtcNow.ToLongDateString()}).");
                        action().Wait();
                        telemetry.TrackEvent($"Job {action.Method.Name} ended ({DateTime.UtcNow.ToLongDateString()}).");;
                    }
                    catch (Exception e)
                    {
                        telemetry.TrackException(e);
                    }
                    finally
                    {
                        telemetry.Flush();
                    }
                }
            });
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var user = GetUser();
            if (context.Exception == null && !string.IsNullOrEmpty(user) && 
                context.ActionDescriptor != null && context.ActionDescriptor.FilterDescriptors != null &&
                !context.ActionDescriptor.FilterDescriptors.Any(c => c.Filter.ToString() != "Microsoft.AspNetCore.Mvc.Authorization.AllowAnonymousFilter"))
            {
                if (context.Result is ObjectResult && ((ObjectResult)context.Result).Value != null)
                    context.Result = new JsonResult(new { jwt = GenerateToken(user), data = ((ObjectResult)context.Result).Value });
                else
                    context.Result = new JsonResult(new { jwt = GenerateToken(user) });
            }
            else if (context.Exception != null)
            {
                if (!(context.Exception is BusinessException))
                {
                    TelemetryClient telemetry = new TelemetryClient();
                    if (!string.IsNullOrEmpty(user))
                        telemetry.Context.User.Id = user;
                    telemetry.TrackException(context.Exception);
                    telemetry.Flush();
                }

                context.ExceptionHandled = true;
                if (context.Exception is BusinessException)
                    context.Result = new BadRequestObjectResult(new { error = context.Exception.Message });
                else if (context.Exception is NotFoundException)
                    context.Result = new NotFoundObjectResult(new { error = context.Exception.Message });
                else if (context.Exception is UnauthorizedException)
                    context.Result = new JsonResult(new { error = context.Exception.Message })
                    {
                        StatusCode = 401
                    };
                else
                    context.Result = new JsonResult(new { error = "Unexpected error." })
                    {
                        StatusCode = 500
                    };
            }
            base.OnActionExecuted(context);
        }

        protected string GenerateToken(string email, int expirationMinutes = 2628000)
        {
            var unixTimestamp = Convert.ToInt64((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, unixTimestamp.ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, (unixTimestamp + (expirationMinutes * 60)).ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: Startup.Configuration.GetSection("Url:Api").Get<string>(),
                audience: Startup.Configuration.GetSection("Url:Web").Get<List<string>>().First(),
                claims: claims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.Configuration.GetSection("Auth:Secret").Get<string>())), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        protected bool IsValidRecaptcha(string recaptchaResponse)
        {
            string url = $"https://www.google.com/recaptcha/api/siteverify?secret={Startup.Configuration.GetSection("RecaptchaSecret").Get<string>()}&response={recaptchaResponse}";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                using (HttpResponseMessage response = client.PostAsync(url, null).Result)
                {
                    RecaptchaResponse result = JsonConvert.DeserializeObject<RecaptchaResponse>(response.Content.ReadAsStringAsync().Result);
                    return result != null && result.Success;
                }
            }
        }

        protected UserBusiness UserBusiness { get { return new UserBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected WalletBusiness WalletBusiness { get { return new WalletBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected ActionBusiness ActionBusiness { get { return new ActionBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected PasswordRecoveryBusiness PasswordRecoveryBusiness { get { return new PasswordRecoveryBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected AdvisorBusiness AdvisorBusiness { get { return new AdvisorBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected FollowBusiness FollowBusiness { get { return new FollowBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected FollowAssetBusiness FollowAssetBusiness { get { return new FollowAssetBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected FollowAdvisorBusiness FollowAdvisorBusiness { get { return new FollowAdvisorBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected AssetBusiness AssetBusiness { get { return new AssetBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected AssetValueBusiness AssetValueBusiness { get { return new AssetValueBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected ExchangeApiAccessBusiness ExchangeApiAccessBusiness { get { return new ExchangeApiAccessBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected AssetCurrentValueBusiness AssetCurrentValueBusiness { get { return new AssetCurrentValueBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected EmailBusiness EmailBusiness { get { return new EmailBusiness(Startup.Configuration, ServiceProvider); } }
        protected EarlyAccessEmailBusiness EarlyAccessEmailBusiness { get { return new EarlyAccessEmailBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected AgencyBusiness AgencyBusiness { get { return new AgencyBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected AgencyRatingBusiness AgencyRatingBusiness { get { return new AgencyRatingBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected ReportBusiness ReportBusiness { get { return new ReportBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected AssetEventBusiness AssetEventBusiness { get { return new AssetEventBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected AssetEventCategoryBusiness AssetEventCategoryBusiness { get { return new AssetEventCategoryBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected AzureStorageBusiness AzureStorageBusiness { get { return new AzureStorageBusiness(Startup.Configuration, ServiceProvider); } }
        protected NewsBusiness NewsBusiness { get { return new NewsBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected ExchangeBusiness ExchangeBusiness { get { return new ExchangeBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected PairBusiness PairBusiness { get { return new PairBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected OrderBusiness OrderBusiness { get { return new OrderBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected AdvisorRankingBusiness AdvisorRankingBusiness { get { return new AdvisorRankingBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected AdvisorProfitBusiness AdvisorProfitBusiness { get { return new AdvisorProfitBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected AdvisorRankingHistoryBusiness AdvisorRankingHistoryBusiness { get { return new AdvisorRankingHistoryBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected AdvisorProfitHistoryBusiness AdvisorProfitHistoryBusiness { get { return new AdvisorProfitHistoryBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected AdvisorMonthlyRankingBusiness AdvisorMonthlyRankingBusiness { get { return new AdvisorMonthlyRankingBusiness(Startup.Configuration, ServiceProvider, ServiceScopeFactory, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
        protected class OnlyAdminAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext context)
            {
                var user = ((BaseController)context.Controller).GetUser();
                if (Startup.Configuration.GetSection("Admins").Get<List<string>>()?.Contains(user) == true)
                    base.OnActionExecuting(context);
                else
                    context.Result = new UnauthorizedResult();
            }
        }
    }
}
