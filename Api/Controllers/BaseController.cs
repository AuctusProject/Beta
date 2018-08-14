using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Auctus.Util.NotShared;
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

namespace Api.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ILoggerFactory LoggerFactory;
        protected readonly ILogger Logger;
        protected readonly Cache MemoryCache;
        protected readonly IServiceProvider ServiceProvider;

        protected BaseController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider)
        {
            MemoryCache = cache;
            LoggerFactory = loggerFactory;
            Logger = loggerFactory.CreateLogger(GetType().Namespace);
            ServiceProvider = serviceProvider;
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

        protected new OkObjectResult Ok()
        {
            return Ok(new { });
        }

        protected new BadRequestObjectResult BadRequest()
        {
            return BadRequest(new { });
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
            base.OnActionExecuted(context);
        }

        protected string GenerateToken(string email, int expirationMinutes = 4320)
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
                audience: Startup.Configuration.GetSection("Url:Web").Get<string>(),
                claims: claims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.Configuration.GetSection("Auth:Secret").Get<string>())), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        protected UserBusiness UserBusiness { get { return new UserBusiness(Startup.Configuration, ServiceProvider, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected WalletBusiness WalletBusiness { get { return new WalletBusiness(Startup.Configuration, ServiceProvider, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected ActionBusiness ActionBusiness { get { return new ActionBusiness(Startup.Configuration, ServiceProvider, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected PasswordRecoveryBusiness PasswordRecoveryBusiness { get { return new PasswordRecoveryBusiness(Startup.Configuration, ServiceProvider, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected AdvisorBusiness AdvisorBusiness { get { return new AdvisorBusiness(Startup.Configuration, ServiceProvider, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected AdviceBusiness AdviceBusiness { get { return new AdviceBusiness(Startup.Configuration, ServiceProvider, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected FollowBusiness FollowBusiness { get { return new FollowBusiness(Startup.Configuration, ServiceProvider, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected FollowAssetBusiness FollowAssetBusiness { get { return new FollowAssetBusiness(Startup.Configuration, ServiceProvider, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected FollowAdvisorBusiness FollowAdvisorBusiness { get { return new FollowAdvisorBusiness(Startup.Configuration, ServiceProvider, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected AssetBusiness AssetBusiness { get { return new AssetBusiness(Startup.Configuration, ServiceProvider, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected AssetValueBusiness AssetValueBusiness { get { return new AssetValueBusiness(Startup.Configuration, ServiceProvider, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected ExchangeApiAccessBusiness ExchangeApiAccessBusiness { get { return new ExchangeApiAccessBusiness(Startup.Configuration, ServiceProvider, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }
        protected RequestToBeAdvisorBusiness RequestToBeAdvisorBusiness { get { return new RequestToBeAdvisorBusiness(Startup.Configuration, ServiceProvider, LoggerFactory, MemoryCache, GetUser(), GetRequestIP()); } }

    }
}
