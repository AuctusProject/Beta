using Auctus.DataAccessInterfaces.Email;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Auctus.Business.Email
{
    public class EmailBusiness 
    {
        private readonly IEmailResource Resource;
        private readonly List<String> EmailErrorList;

        internal EmailBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider)
        {
            Resource = (IEmailResource)serviceProvider.GetService(typeof(IEmailResource));
            EmailErrorList = configuration.GetSection("Email:Error").Get<List<string>>();
        }

        public bool IsValidEmail(string strIn)
        {
            if (String.IsNullOrEmpty(strIn))
                return false;

            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            IdnMapping idn = new IdnMapping();
            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                throw new RegexMatchTimeoutException();
            }
            return match.Groups[1].Value + domainName;
        }

        public async Task SendErrorEmailAsync(string message, string subject = "Critical error on Auctus Beta", Exception ex = null)
        {
            await SendAsync(EmailErrorList, subject, string.Format("{0}<br/><br/><br/>{1}", message, ex?.ToString()));
        }

        public async Task SendAsync(IEnumerable<string> to, string subject, string body, bool bodyIsHtml = true, string from = "noreply@auctus.org",
            IEnumerable<string> cc = null, IEnumerable<string> bcc = null, IEnumerable<SendGrid.Helpers.Mail.Attachment> attachment = null)
        {
            await Resource.SendAsync(to, subject, body, bodyIsHtml, from, cc, bcc, attachment);
        }
    }
}
