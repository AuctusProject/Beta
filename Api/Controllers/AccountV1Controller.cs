﻿using Api.Model.Account;
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
        public async new Task<IActionResult> Register([FromBody]RegisterRequest registerRequest)
        {
            return await base.Register(registerRequest);
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
        public new async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordRequest forgotPasswordRequest)
        {
            return await base.ForgotPassword(forgotPasswordRequest);
        }

        [Route("me/passwords")]
        [HttpPut]
        [AllowAnonymous]
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
        public async new Task<IActionResult> ResendEmailConfirmation()
        {
            return await base.ResendEmailConfirmation();
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

        [Route("search/{term}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult Search([FromRoute]string term)
        {
            return base.Search(term);
        }


        [Route("admin")]
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
