using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Model.Account
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
        public string Captcha { get; set; }
    }
}
