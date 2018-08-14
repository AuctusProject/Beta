using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Linq;
using System.Text;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading.Tasks;

namespace Auctus.Util
{
    public static class Email
    {
        public static bool IsValidEmail(string strIn)
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

        private static string DomainMapper(Match match)
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

        public static async Task SendErrorEmailAsync(string sendGridKey, IEnumerable<string> emailErrorList, string message, Exception ex = null)
        {
            await SendAsync(sendGridKey, emailErrorList, "Critical error on Auctus Beta", string.Format("{0}<br/><br/><br/>{1}", message, ex?.ToString()));
        }

        public static async Task SendAsync(string sendGridKey, IEnumerable<string> to, string subject, string body, bool bodyIsHtml = true, string from = "noreply@auctus.org", 
            IEnumerable<string> cc = null, IEnumerable<string> bcc = null, IEnumerable<SendGrid.Helpers.Mail.Attachment> attachment = null)
        {
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentNullException("subject");

            if (to == null || !to.Any())
                throw new ArgumentNullException("to");

            List<EmailAddress> toList = new List<EmailAddress>();

            foreach (string t in to)
                toList.Add(new EmailAddress(t));          

            var mailMessage = MailHelper.CreateSingleEmailToMultipleRecipients(new EmailAddress(from, "Auctus Mail Service"), toList, subject, bodyIsHtml ? null : body, bodyIsHtml ? body : null);
            
            if (attachment != null)
            {
                foreach (SendGrid.Helpers.Mail.Attachment a in attachment)
                    mailMessage.Attachments.Add(a);
            }

            if (cc != null)
            {
                foreach (string c in cc)
                    mailMessage.AddCc(c);
            }

            if (bcc != null)
            {
                foreach (string b in bcc)
                    mailMessage.AddBcc(b);
            }

            SendGridClient client = new SendGridClient(sendGridKey);
            await client.SendEmailAsync(mailMessage);
        }
    }
}
