using Auctus.DataAccess.Advisor;
using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
using Auctus.Model;
using Auctus.Util;
using Auctus.Util.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
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
            if (string.IsNullOrWhiteSpace(LoggedEmail))
                return null;

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
            var user = UserBusiness.GetById(request.UserId);
            if (user.IsAdvisor)
                throw new BusinessException("User is already an Expert.");
            if (request.Approved == true)
                throw new BusinessException("Request is already approved.");

            request.Approved = true;
            var urlGuid = request.UrlGuid ?? Guid.NewGuid();
            var advisor = AdvisorBusiness.CreateFromRequest(request, urlGuid);
            if (!request.UrlGuid.HasValue)
                await AzureStorageBusiness.UploadUserPictureFromBytesAsync($"{urlGuid}.png", AdvisorBusiness.GetNoUploadedImageForAdvisor(user));
            using (var transaction = TransactionalDapperCommand)
            {
                transaction.Insert(advisor);
                transaction.Update(request);
                transaction.Commit();
            }

            UserBusiness.ClearUserCache(user.Email);
            AdvisorBusiness.UpdateAdvisorsCacheAsync();

            await SendRequestApprovedNotificationAsync(user);
        }

        public async Task RejectAsync(int id)
        {
            var request = Data.GetById(id);
            var user = UserBusiness.GetById(request.UserId);

            request.Approved = false;
            Update(request);

            UserBusiness.ClearUserCache(user.Email);

            await SendRequestRejectedNotificationAsync(user);
        }

        public async Task<LoginResponse> CreateAsync(string email, string password, string name, string description, string previousExperience,
            bool changePicture, Stream pictureStream, string pictureExtension)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessException("Name must be filled.");
            if (name.Length > 50)
                throw new BusinessException("Name cannot have more than 50 characters.");
            if (string.IsNullOrWhiteSpace(description))
                throw new BusinessException("Short description must be filled.");
            if (description.Length > 160)
                throw new BusinessException("Short description cannot have more than 160 characters.");
            if (string.IsNullOrWhiteSpace(previousExperience))
                throw new BusinessException("Previous experience must be filled.");
            if (previousExperience.Length > 4000)
                throw new BusinessException("Previous experience cannot have more than 4000 characters.");

            byte[] picture = null;
            if (changePicture && pictureStream != null)
                picture = AdvisorBusiness.GetPictureBytes(pictureStream, pictureExtension);

            RequestToBeAdvisor request = null;
            User user = null;
            if (LoggedEmail != null)
            {
                if (!string.IsNullOrWhiteSpace(email) && email != LoggedEmail)
                    throw new BusinessException("Invalid email.");
                if (!string.IsNullOrWhiteSpace(password))
                    throw new BusinessException("Invalid password.");

                user = UserBusiness.GetForLoginByEmail(LoggedEmail);
                if (user == null)
                    throw new NotFoundException("User not found.");
                if (user.IsAdvisor)
                    throw new BusinessException("User was already approved as Expert.");

                request = GetByUser(user.Id);
                if (request?.Approved == true)
                    throw new BusinessException("Request was already approved.");
            }
            else
                user = UserBusiness.GetValidUserToRegister(email, password, null);

            Guid? urlGuid = null;
            if (picture != null)
            {
                urlGuid = Guid.NewGuid();
                if (await AzureStorageBusiness.UploadUserPictureFromBytesAsync($"{urlGuid}.png", picture) && request != null && request.UrlGuid.HasValue)
                    await AzureStorageBusiness.DeleteUserPicture($"{request.UrlGuid.Value}.png");
            }
            else if (!changePicture)
                urlGuid = request?.UrlGuid;

            RequestToBeAdvisor newRequest = null;
            using (var transaction = TransactionalDapperCommand)
            {
                if (LoggedEmail == null)
                    transaction.Insert(user);

                newRequest = new RequestToBeAdvisor()
                {
                    CreationDate = Data.GetDateTimeNow(),
                    Name = name,
                    Description = description,
                    PreviousExperience = previousExperience,
                    UserId = user.Id,
                    UrlGuid = urlGuid
                };
                transaction.Insert(newRequest);
                transaction.Commit();
            }
            if (LoggedEmail == null)
                await UserBusiness.SendEmailConfirmationAsync(user.Email, user.ConfirmationCode);

            await SendRequestToBeAdvisorEmailAsync(user, newRequest, request);

            bool hasInvestment = UserBusiness.GetUserHasInvestment(user, out decimal? aucAmount);
            return new LoginResponse()
            {
                Id = user.Id,
                Email = user.Email,
                PendingConfirmation = !user.ConfirmationDate.HasValue,
                HasInvestment = hasInvestment,
                IsAdvisor = false,
                RequestedToBeAdvisor = true
            };
        }
        private async Task SendRequestToBeAdvisorEmailAsync(User user, RequestToBeAdvisor newRequestToBeAdvisor, RequestToBeAdvisor oldRequestToBeAdvisor)
        {
            await EmailBusiness.SendErrorEmailAsync(string.Format(@"
<a target='_blank' href='{0}/advisors-requests'>Link to approve/reject</a>
<br/>
<br/>
Email: {1} 
<br/>
<br/>
<b>Old Name</b>: {2}
<br/>
<b>New Name</b>: {3}
<br/>
<br/>
<b>Old Description</b>: {4}
<br/>
<b>New Description</b>: {5}
<br/>
<br/>
<b>Old Previous Experience</b>: {6}
<br/>
<b>New Previous Experience</b>: {7}", WebUrl, user.Email, 
oldRequestToBeAdvisor?.Name ?? "N/A", newRequestToBeAdvisor.Name,
oldRequestToBeAdvisor?.Description ?? "N/A", newRequestToBeAdvisor.Description,
oldRequestToBeAdvisor?.PreviousExperience ?? "N/A", newRequestToBeAdvisor.PreviousExperience),
string.Format("[{0}] Request to be adivosr - Auctus Beta", oldRequestToBeAdvisor == null ? "NEW" : "UPDATE"));
        }

        private async Task SendRequestRejectedNotificationAsync(User user)
        {
            await EmailBusiness.SendUsingTemplateAsync(new string[] { user.Email },
                "Your request to become an expert was rejected - Auctus Beta",
                "<p>We are sorry to inform you that at this moment your request to become an expert can not be accepted.</p>",
                EmailTemplate.NotificationType.BecomeAdvisor);
        }

        private async Task SendRequestApprovedNotificationAsync(User user)
        {
            await EmailBusiness.SendUsingTemplateAsync(new string[] { user.Email },
                "Your request to become an expert was approved! - Auctus Beta",
                $@"<p>We are happy to inform you that your request to become an Expert on Auctus Platform was approved. To start recommending assets now, <a href='{WebUrl}/expert-details/{user.Id}' target='_blank'>click here</a>.</p>",
                EmailTemplate.NotificationType.BecomeAdvisor);
        }
    }
}
