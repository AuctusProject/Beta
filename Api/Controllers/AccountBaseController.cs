using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Model.Account;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Web3;
using Auctus.Model;
using Auctus.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    public class AccountBaseController : BaseController
    {
        protected AccountBaseController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory) :
            base(loggerFactory, cache, serviceProvider, serviceScopeFactory) { }

        protected virtual IActionResult ValidateSignature(ValidateSignatureRequest signatureRequest)
        {
            if (signatureRequest == null)
                return BadRequest();
            
            return Ok(UserBusiness.ValidateSignature(signatureRequest.Address, signatureRequest.Signature));
        }

        protected virtual IActionResult Login(LoginRequest loginRequest)
        {
            if (loginRequest == null)
                return BadRequest();
            if (!IsValidRecaptcha(loginRequest.Captcha))
                return BadRequest(new { error = "Invalid Captcha." });

            var loginResponse = UserBusiness.Login(loginRequest.Email, loginRequest.Password);
            return Ok(new { logged = !loginResponse.PendingConfirmation, jwt = GenerateToken(loginRequest.Email.ToLower().Trim()), data = loginResponse });
        }

        protected virtual IActionResult GetLoginData()
        {
            return Ok(UserBusiness.GetLoginResponse());
        }

        protected virtual IActionResult SocialLogin(SocialLoginRequest socialLoginRequest)
        {
            if (socialLoginRequest == null)
                return BadRequest();

            var loginResponse = UserBusiness.SocialLogin(SocialNetworkType.Get(socialLoginRequest.SocialNetworkType), socialLoginRequest.Email, socialLoginRequest.Token, socialLoginRequest.RequestedToBeAdvisor);
            return Ok(new { logged = !loginResponse.PendingConfirmation, jwt = GenerateToken(socialLoginRequest.Email.ToLower().Trim()), data = loginResponse });
        }

        protected virtual IActionResult RecoverPassword(RecoverPasswordRequest recoverPasswordRequest)
        {
            if (recoverPasswordRequest == null)
                return BadRequest();

            var loginResponse = PasswordRecoveryBusiness.RecoverPassword(recoverPasswordRequest.Code, recoverPasswordRequest.Password);
            return Ok(new { jwt = GenerateToken(loginResponse.Email), data = loginResponse });
        }

        protected virtual async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest)
        {
            if (forgotPasswordRequest == null)
                return BadRequest();
            if (!IsValidRecaptcha(forgotPasswordRequest.Captcha))
                return BadRequest(new { error = "Invalid Captcha." });

            await PasswordRecoveryBusiness.SendEmailForForgottenPasswordAsync(forgotPasswordRequest.Email);
            return Ok();
        }

        protected virtual IActionResult ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            if (changePasswordRequest == null)
                return BadRequest();

            UserBusiness.ChangePassword(changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword);
            return Ok();
        }

        protected virtual async Task<IActionResult> RegisterAsync(RegisterRequest registerRequest)
        {
            if (registerRequest == null)
                return BadRequest();
            if (!IsValidRecaptcha(registerRequest.Captcha))
                return BadRequest(new { error = "Invalid Captcha." });

            var loginResponse = await UserBusiness.RegisterAsync(registerRequest.Email, registerRequest.Password, registerRequest.ReferralCode);
            return Ok(new { logged = !loginResponse.PendingConfirmation, jwt = GenerateToken(registerRequest.Email.ToLower().Trim()), data = loginResponse });
        }

        protected virtual async Task<IActionResult> ResendEmailConfirmationAsync()
        {
            var loginResponse = await UserBusiness.ResendEmailConfirmationAsync();
            return Ok(new { logged = !loginResponse.PendingConfirmation, jwt = GenerateToken(loginResponse.Email), data = loginResponse });
        }

        protected virtual IActionResult ConfirmEmail(ConfirmEmailRequest confirmEmailRequest)
        {
            var loginResponse = UserBusiness.ConfirmEmail(confirmEmailRequest.Code);
            return Ok(new { logged = true, jwt = GenerateToken(loginResponse.Email), data = loginResponse });
        }

        protected virtual IActionResult ListAdvices(int? top, int? lastAdviceId)
        {
            return Ok(AdviceBusiness.ListFeed(top, lastAdviceId));
        }

        protected virtual IActionResult SetReferralCode(SetReferralRequest setReferralRequest)
        {
            if(setReferralRequest == null)
                return BadRequest();

            return Ok(UserBusiness.SetReferralCode(setReferralRequest.ReferralCode));
        }

        protected virtual IActionResult GetReferralProgramInfo()
        {
            return Ok(UserBusiness.GetReferralProgramInfo());
        }

        protected virtual IActionResult GetWalletLoginInfo()
        {
            return Ok(UserBusiness.GetWalletLoginInfo());
        }

        protected virtual IActionResult GetAUCAmount(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return BadRequest();

            return Ok(WalletBusiness.GetAucAmount(address));
        }

        protected virtual IActionResult IsValidReferralCode(string referralCode)
        {
            if (String.IsNullOrWhiteSpace(referralCode) || referralCode.Length != 7)
                return BadRequest();

            return Ok(UserBusiness.IsValidReferralCode(referralCode));
        }

        protected virtual IActionResult SetConfiguration(SetConfigurationRequest setConfigurationRequest)
        {
            if (setConfigurationRequest == null)
                return BadRequest();

            UserBusiness.SetConfiguration(setConfigurationRequest.AllowNotifications);
            return Ok();
        }

        protected virtual IActionResult GetConfiguration()
        {
            return Ok(UserBusiness.GetConfiguration());
        }

        protected virtual IActionResult Search(string term)
        {
            return Ok(UserBusiness.Search(term));
        }

        protected virtual IActionResult GetDashboard()
        {
            return Ok(ActionBusiness.GetDashboardData());
        }

        protected virtual IActionResult RequestEarlyAccess(EarlyAccessRequest earlyAccessRequest)
        {
            EarlyAccessEmailBusiness.Create(earlyAccessRequest.Name, earlyAccessRequest.Email, earlyAccessRequest.Twitter);
            return Ok();
        }
    }
}