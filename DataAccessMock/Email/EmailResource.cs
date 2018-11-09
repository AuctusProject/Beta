using Auctus.DataAccessInterfaces.Email;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Email
{
    public class EmailResource : IEmailResource
    {
        public Task SendAsync(IEnumerable<string> to, string subject, string body, bool bodyIsHtml = true, string from = "noreply@auctus.org", IEnumerable<string> cc = null, IEnumerable<string> bcc = null, IEnumerable<Attachment> attachment = null)
        {
            return Task.FromResult(0);
        }

        Task IEmailResource.IncludeSubscribedEmailFromWebsite(string email, string firstName, string lastName)
        {
            return Task.FromResult(0);
        }
    }
}
