using Auctus.DataAccessInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.DataAccessInterfaces.Email
{
    public interface IEmailResource 
    {
        Task SendAsync(IEnumerable<string> to, string subject, string body, bool bodyIsHtml = true, string from = "noreply@auctus.org",
            IEnumerable<string> cc = null, IEnumerable<string> bcc = null, IEnumerable<SendGrid.Helpers.Mail.Attachment> attachment = null);

        Task IncludeSubscribedEmailFromWebsite(string email, string firstName, string lastName);
    }
}
