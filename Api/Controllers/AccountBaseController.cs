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

            var loginResponse = UserBusiness.Login(loginRequest.Email, loginRequest.Password);
            return Ok(new { logged = !loginResponse.PendingConfirmation, jwt = GenerateToken(loginRequest.Email.ToLower().Trim()), data = loginResponse });
        }

        protected virtual IActionResult RecoverPassword(RecoverPasswordRequest recoverPasswordRequest)
        {
            if (recoverPasswordRequest == null)
                return BadRequest();

            PasswordRecoveryBusiness.RecoverPassword(recoverPasswordRequest.Code, recoverPasswordRequest.Password);
            return Ok();
        }

        protected virtual async Task<IActionResult> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            if (forgotPasswordRequest == null || String.IsNullOrWhiteSpace(forgotPasswordRequest.Email))
                return BadRequest();

            await PasswordRecoveryBusiness.SendEmailForForgottenPassword(forgotPasswordRequest.Email);
            return Ok();
        }

        protected virtual IActionResult ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            if (changePasswordRequest == null || String.IsNullOrWhiteSpace(changePasswordRequest.CurrentPassword) || String.IsNullOrWhiteSpace(changePasswordRequest.NewPassword))
                return BadRequest();

            UserBusiness.ChangePassword(changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword);
            return Ok();
        }

        protected virtual async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            if (registerRequest == null)
                return BadRequest();

            var loginResponse = await UserBusiness.Register(registerRequest.Email, registerRequest.Password, registerRequest.ReferralCode, registerRequest.RequestedToBeAdvisor);
            return Ok(new { logged = !loginResponse.PendingConfirmation, jwt = GenerateToken(registerRequest.Email.ToLower().Trim()), data = loginResponse });
        }

        protected virtual async Task<IActionResult> ResendEmailConfirmation()
        {
            await UserBusiness.ResendEmailConfirmation();
            return Ok();
        }

        protected virtual IActionResult ConfirmEmail(ConfirmEmailRequest confirmEmailRequest)
        {
            return Ok(UserBusiness.ConfirmEmail(confirmEmailRequest.Code));
        }

        protected virtual IActionResult ListAdvices(int? top, int? lastAdviceId)
        {
            return Ok(AdviceBusiness.ListFeed(top, lastAdviceId));
        }

        protected virtual IActionResult SetReferralCode(SetReferralRequest setReferralRequest)
        {
            if(setReferralRequest == null || String.IsNullOrWhiteSpace(setReferralRequest.ReferralCode))
                return BadRequest();

            UserBusiness.SetReferralCode(setReferralRequest.ReferralCode);
            return Ok();
        }

        protected virtual IActionResult GetReferralProgramInfo()
        {
            return Ok(UserBusiness.GetReferralProgramInfo());
        }

        protected virtual IActionResult SetAllowNotifications(SetAllowNotificationsRequest setAllowNotificationsRequest)
        {
            if (setAllowNotificationsRequest == null)
                return BadRequest();

            UserBusiness.SetAllowNotifications(setAllowNotificationsRequest.AllowNotifications);
            return Ok();
        }

        protected virtual IActionResult GetAllowNotifications()
        {
            return Ok(new { allow = UserBusiness.GetValidUser().AllowNotifications });
        }

        protected virtual IActionResult Search(string term)
        {
            return Ok(UserBusiness.Search(term));
        }
    }
}