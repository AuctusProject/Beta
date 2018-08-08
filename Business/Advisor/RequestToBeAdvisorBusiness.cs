using Auctus.DataAccess.Advisor;
using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
using Auctus.Util;
using Auctus.Util.NotShared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.Business.Advisor
{
    public class RequestToBeAdvisorBusiness : BaseBusiness<RequestToBeAdvisor, IRequestToBeAdvisorData<RequestToBeAdvisor>>
    {
        public RequestToBeAdvisorBusiness(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(serviceProvider, loggerFactory, cache, email, ip) { }

        public RequestToBeAdvisor GetByUser(int userId)
        {
            return Data.GetByUser(userId);
        }

        public RequestToBeAdvisor GetByEmail(string email)
        {
            var user = UserBusiness.GetByEmail(email);
            if(user == null)
                throw new ArgumentException("Invalid email.");
            return Data.GetByUser(user.Id);
        }

        public async Task<RequestToBeAdvisor> Create(string email, string name, string description, string previousExperience)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name must be filled.");
            if (name.Length > 100)
                throw new ArgumentException("Name cannot have more than 100 characters.");
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description must be filled.");
            if (description.Length > 4000)
                throw new ArgumentException("Description cannot have more than 4000 characters.");
            if (string.IsNullOrWhiteSpace(previousExperience))
                throw new ArgumentException("Previous experience must be filled.");
            if (previousExperience.Length > 4000)
                throw new ArgumentException("Previous experience cannot have more than 4000 characters.");

            var user = GetValidUser();
            var request = GetByUser(user.Id);
            if (request?.Approved ?? false)
                throw new ArgumentException("User was already approved as advisor.");

            var newRequest = new RequestToBeAdvisor()
            {
                CreationDate = DateTime.UtcNow,
                Name = name,
                Description = description,
                PreviousExperience = previousExperience,
                UserId = user.Id
            };
            Data.Insert(newRequest);
            await SendRequestToBeAdvisorEmail(user, newRequest, request);
            return newRequest;
        }
        private async Task SendRequestToBeAdvisorEmail(User user, RequestToBeAdvisor newRequestToBeAdvisor, RequestToBeAdvisor oldRequestToBeAdvisor)
        {
            await Email.SendAsync(Config.EMAIL_FOR_CRITICAL_ERROR,
                string.Format("[{0}] Request to be adivosr - Auctus Beta", oldRequestToBeAdvisor == null ? "NEW" : "UPDATE"),
                string.Format(@"Email: {0} 
<br/>
<br/>
<b>Old Name</b>: {1}
<br/>
<b>New Name</b>: {2}
<br/>
<br/>
<b>Old Description</b>: {3}
<br/>
<b>New Description</b>: {4}
<br/>
<br/>
<b>Old Previous Experience</b>: {5}
<br/>
<b>New Previous Experience</b>: {6}", user.Email, 
oldRequestToBeAdvisor?.Name ?? "N/A", newRequestToBeAdvisor.Name,
oldRequestToBeAdvisor?.Description ?? "N/A", newRequestToBeAdvisor.Description,
oldRequestToBeAdvisor?.PreviousExperience ?? "N/A", newRequestToBeAdvisor.PreviousExperience));
        }    }
}
