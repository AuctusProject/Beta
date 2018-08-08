using Auctus.DataAccessInterfaces;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Account
{
    public class UserData : BaseData<User>, IUserData<User>
    {
        public User GetByConfirmationCode(string code)
        {
            throw new NotImplementedException();
        }

        public User GetByEmail(string email)
        {
            return new DomainObjects.Advisor.Advisor()
            {
                Id = 1,
                Email = "test@auctus.org",
                CreationDate = DateTime.UtcNow.AddDays(-7),
                ConfirmationDate = DateTime.UtcNow.AddDays(-5),
                ConfirmationCode = "",
                Password = "",
                IsAdvisor = true,
                Name = "Tester Advisor",
                Description = "Test Advisor description",
                Enabled = true,
                BecameAdvisorDate = DateTime.UtcNow.AddDays(-5),
                Wallet = new Wallet()
                {
                    Id = 1,
                    UserId = 1,
                    Address = "0x0000000000000000000000000000000000000000",
                    CreationDate = DateTime.UtcNow.AddDays(-5)
                },
                RequestToBeAdvisor = new DomainObjects.Advisor.RequestToBeAdvisor()
                {
                    Id = 1,
                    UserId = 1,
                    Approved = true,
                    CreationDate = DateTime.UtcNow.AddDays(-5),
                    Description = "Test Advisor description",
                    Name = "Tester Advisor",
                    PreviousExperience = ""
                }
            };
        }

        public User GetForLogin(string email)
        {
            throw new NotImplementedException();
        }

        public User GetForNewWallet(string email)
        {
            throw new NotImplementedException();
        }
    }
}
