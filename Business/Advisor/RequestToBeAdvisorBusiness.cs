using Auctus.DataAccess.Advisor;
using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
using Auctus.Util;
using Auctus.Util.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.Business.Advisor
{
    public class RequestToBeAdvisorBusiness : BaseBusiness<RequestToBeAdvisor, IRequestToBeAdvisorData<RequestToBeAdvisor>>
    {
        public RequestToBeAdvisorBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, ILoggerFactory loggerFactory, Cache cache, string email, string ip) : base(configuration, serviceProvider, serviceScopeFactory, loggerFactory, cache, email, ip) { }

        public RequestToBeAdvisor GetByUser(int userId)
        {
            return Data.GetByUser(userId);
        }

        public RequestToBeAdvisor GetByLoggedEmail()
        {
            var user = UserBusiness.GetByEmail(LoggedEmail);
            if(user == null)
                throw new NotFoundException("Invalid email.");
            return Data.GetByUser(user.Id);
        }

        public List<RequestToBeAdvisor> ListPending()
        {
            return Data.ListPending();
        }

        public async Task ApproveAsync(int id)
        {
            var request = Data.GetById(id);
            var user = UserBusiness.GetById(id);
            if (user.IsAdvisor)
                throw new BusinessException("User is already advisor.");
            if (request.Approved == true)
                throw new BusinessException("Requets is already approved.");

            request.Approved = true;
            var advisor = AdvisorBusiness.CreateFromRequest(request);
            using (var transaction = TransactionalDapperCommand)
            {
                transaction.Insert(advisor);
                transaction.Update(request);
                transaction.Commit();
            }
            await AzureStorageBusiness.UploadUserPictureFromBytesAsync($"{user.Id}.png", UserBusiness.GetNoUploadedImageForUser(user));
        }

        public void Reject(int id)
        {
            var request = Data.GetById(id);
            request.Approved = false;
            Update(request);
        }

        public async Task<RequestToBeAdvisor> CreateAsync(string name, string description, string previousExperience)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessException("Name must be filled.");
            if (name.Length > 100)
                throw new BusinessException("Name cannot have more than 100 characters.");
            if (string.IsNullOrWhiteSpace(description))
                throw new BusinessException("Description must be filled.");
            if (description.Length > 4000)
                throw new BusinessException("Description cannot have more than 4000 characters.");
            if (string.IsNullOrWhiteSpace(previousExperience))
                throw new BusinessException("Previous experience must be filled.");
            if (previousExperience.Length > 4000)
                throw new BusinessException("Previous experience cannot have more than 4000 characters.");

            var user = GetValidUser();
            var request = GetByUser(user.Id);
            if (request?.Approved == true)
                throw new BusinessException("User was already approved as advisor.");

            var newRequest = new RequestToBeAdvisor()
            {
                CreationDate = Data.GetDateTimeNow(),
                Name = name,
                Description = description,
                PreviousExperience = previousExperience,
                UserId = user.Id
            };
            Data.Insert(newRequest);
            await SendRequestToBeAdvisorEmailAsync(user, newRequest, request);
            return newRequest;
        }
        private async Task SendRequestToBeAdvisorEmailAsync(User user, RequestToBeAdvisor newRequestToBeAdvisor, RequestToBeAdvisor oldRequestToBeAdvisor)
        {
            await EmailBusiness.SendErrorEmailAsync(string.Format(@"Email: {0} 
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
oldRequestToBeAdvisor?.PreviousExperience ?? "N/A", newRequestToBeAdvisor.PreviousExperience),
string.Format("[{0}] Request to be adivosr - Auctus Beta", oldRequestToBeAdvisor == null ? "NEW" : "UPDATE"));
        }
    }
}
