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
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    public class AccountBaseController : BaseController
    {
        protected AccountBaseController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider) : base(loggerFactory, cache, serviceProvider) { }

        protected virtual IActionResult ValidateSignature(ValidateSignatureRequest signatureRequest)
        {
            if (signatureRequest == null)
                return BadRequest();
            
            try
            {
                return Ok(UserBusiness.ValidateSignature(signatureRequest.Address, signatureRequest.Signature));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        protected virtual IActionResult Login(LoginRequest loginRequest)
        {
            if (loginRequest == null)
                return BadRequest();

            try
            {
                var loginResponse = UserBusiness.Login(loginRequest.Email, loginRequest.Password);
                return Ok(new { logged = !loginResponse.PendingConfirmation, jwt = GenerateToken(loginRequest.Email.ToLower().Trim()), data = loginResponse });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        protected virtual IActionResult RecoverPassword(RecoverPasswordRequest recoverPasswordRequest)
        {
            if (recoverPasswordRequest == null)
                return BadRequest();

            try
            {
                PasswordRecoveryBusiness.RecoverPassword(recoverPasswordRequest.Code, recoverPasswordRequest.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        protected virtual async Task<IActionResult> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            if (forgotPasswordRequest == null || String.IsNullOrWhiteSpace(forgotPasswordRequest.Email))
                return BadRequest();

            try
            {
                await PasswordRecoveryBusiness.SendEmailForForgottenPassword(forgotPasswordRequest.Email);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        protected virtual IActionResult ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            if (changePasswordRequest == null || String.IsNullOrWhiteSpace(changePasswordRequest.CurrentPassword) || String.IsNullOrWhiteSpace(changePasswordRequest.NewPassword))
                return BadRequest();

            try
            {
                UserBusiness.ChangePassword(changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        protected virtual async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            if (registerRequest == null)
                return BadRequest();

            try
            {
                var loginResponse = await UserBusiness.Register(registerRequest.Email, registerRequest.Password, registerRequest.RequestedToBeAdvisor);
                return Ok(new { logged = !loginResponse.PendingConfirmation, jwt = GenerateToken(registerRequest.Email.ToLower().Trim()), data = loginResponse });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        protected virtual async Task<IActionResult> ResendEmailConfirmation()
        {
            try
            {
                await UserBusiness.ResendEmailConfirmation();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        protected virtual IActionResult ConfirmEmail(ConfirmEmailRequest confirmEmailRequest)
        {
            try
            {
                var loginResponse = UserBusiness.ConfirmEmail(confirmEmailRequest.Code);
                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        protected virtual IActionResult ToggleFollowAdvisor(int id)
        {
            try
            {
                var followAdvisor = UserBusiness.ToggleFollowAdvisor(id);
                return Ok(followAdvisor);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}