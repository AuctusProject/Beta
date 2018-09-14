using Api.Model.Account;
using Auctus.DomainObjects.Account;
using Auctus.Model;
using Auctus.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/accounts/")]
    [EnableCors("Default")]
    public class AccountV1Controller : AccountBaseController
    {
        public AccountV1Controller(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory) :
            base(loggerFactory, cache, serviceProvider, serviceScopeFactory) { }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async new Task<IActionResult> RegisterAsync([FromBody]RegisterRequest registerRequest)
        {
            return await base.RegisterAsync(registerRequest);
        }

        [HttpPost]
        [Route("social_login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult SocialLogin([FromBody]SocialLoginRequest socialLoginRequest)
        {
            return base.SocialLogin(socialLoginRequest);
        }

        [Route("login")]
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult Login([FromBody]LoginRequest loginRequest)
        {
            return base.Login(loginRequest);
        }

        [Route("passwords/recover")]
        [HttpPut]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult RecoverPassword([FromBody]RecoverPasswordRequest recoverPasswordRequest)
        {
            return base.RecoverPassword(recoverPasswordRequest);
        }

        [Route("passwords/recover")]
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new async Task<IActionResult> ForgotPasswordAsync([FromBody]ForgotPasswordRequest forgotPasswordRequest)
        {
            return await base.ForgotPasswordAsync(forgotPasswordRequest);
        }

        [Route("me/passwords")]
        [HttpPut]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ChangePassword([FromBody]ChangePasswordRequest changePasswordRequest)
        {
            return base.ChangePassword(changePasswordRequest);
        }

        [Route("me/signatures")]
        [HttpPost]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ValidateSignature([FromBody]ValidateSignatureRequest signatureRequest)
        {
            return base.ValidateSignature(signatureRequest);
        }

        [Route("me/confirmations")]
        [HttpGet]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async new Task<IActionResult> ResendEmailConfirmationAsync()
        {
            return await base.ResendEmailConfirmationAsync();
        }

        [Route("me/confirmations")]
        [HttpPost]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ConfirmEmail([FromBody]ConfirmEmailRequest confirmEmailRequest)
        {
            return base.ConfirmEmail(confirmEmailRequest);
        }

        [Route("me/advices")]
        [HttpGet]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult ListAdvices([FromQuery]int? top, [FromQuery]int? lastAdviceId)
        {
            return base.ListAdvices(top, lastAdviceId);
        }

        [Route("me/referrals")]
        [HttpPost]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult SetReferralCode([FromBody]SetReferralRequest setReferralRequest)
        {
            return base.SetReferralCode(setReferralRequest);
        }

        [Route("me/referrals")]
        [HttpGet]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult GetReferralProgramInfo()
        {
            return base.GetReferralProgramInfo();
        }

        [Route("me/wallet_login")]
        [HttpGet]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult GetWalletLoginInfo()
        {
            return base.GetWalletLoginInfo();
        }

        [Route("auc/{address}")]
        [HttpGet]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult GetAUCAmount([FromRoute]string address)
        {
            return base.GetAUCAmount(address);
        }

        [Route("referrals")]
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult IsValidReferralCode([FromQuery]string referralCode)
        {
            return base.IsValidReferralCode(referralCode);
        }

        [Route("me/configuration")]
        [HttpPost]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult SetConfiguration([FromBody]SetConfigurationRequest setConfigurationRequest)
        {
            return base.SetConfiguration(setConfigurationRequest);
        }

        [Route("me/configuration")]
        [HttpGet]
        [Authorize("Bearer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult GetConfiguration()
        {
            return base.GetConfiguration();
        }

        [Route("search")]
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult Search([FromQuery]string term)
        {
            return base.Search(term);
        }


        [Route("dashboard")]
        [HttpGet]
        [Authorize("Bearer")]
        [OnlyAdmin]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult GetDashboard()
        {
            return base.GetDashboard();
        }
    }
}
