using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Account;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auctus.DataAccessMock.Advisor
{
    public class AdvisorData : BaseData<DomainObjects.Advisor.Advisor>, IAdvisorData<DomainObjects.Advisor.Advisor>
    {
        public List<DomainObjects.Advisor.Advisor> ListEnabled()
        {
            var advisors = new List<DomainObjects.Advisor.Advisor>();
            advisors.Add(new DomainObjects.Advisor.Advisor()
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
            });
            advisors.Add(new DomainObjects.Advisor.Advisor()
            {
                Id = 2,
                Email = "test2@auctus.org",
                CreationDate = DateTime.UtcNow.AddDays(-7),
                ConfirmationDate = DateTime.UtcNow.AddDays(-5),
                ConfirmationCode = "",
                Password = "",
                IsAdvisor = true,
                Name = "Tester Advisor 2",
                Description = "Test Advisor 2 description",
                Enabled = true,
                BecameAdvisorDate = DateTime.UtcNow.AddDays(-4),
                Wallet = new Wallet()
                {
                    Id = 2,
                    UserId = 2,
                    Address = "0x0000000000000000000000000000000000000001",
                    CreationDate = DateTime.UtcNow.AddDays(-5)
                },
                RequestToBeAdvisor = new DomainObjects.Advisor.RequestToBeAdvisor()
                {
                    Id = 2,
                    UserId = 2,
                    Approved = true,
                    CreationDate = DateTime.UtcNow.AddDays(-5),
                    Description = "Test Advisor 2 description",
                    Name = "Tester Advisor 2",
                    PreviousExperience = ""
                }
            });
            advisors.Add(new DomainObjects.Advisor.Advisor()
            {
                Id = 3,
                Email = "test3@auctus.org",
                CreationDate = DateTime.UtcNow.AddDays(-7),
                ConfirmationDate = DateTime.UtcNow.AddDays(-5),
                ConfirmationCode = "",
                Password = "",
                IsAdvisor = true,
                Name = "Tester Advisor 3",
                Description = "Test Advisor 3 description",
                Enabled = true,
                BecameAdvisorDate = DateTime.UtcNow.AddDays(-3),
                Wallet = new Wallet()
                {
                    Id = 3,
                    UserId = 3,
                    Address = "0x0000000000000000000000000000000000000002",
                    CreationDate = DateTime.UtcNow.AddDays(-5)
                },
                RequestToBeAdvisor = new DomainObjects.Advisor.RequestToBeAdvisor()
                {
                    Id = 3,
                    UserId = 3,
                    Approved = true,
                    CreationDate = DateTime.UtcNow.AddDays(-5),
                    Description = "Test Advisor 3 description",
                    Name = "Tester Advisor 3",
                    PreviousExperience = ""
                }
            });
            return advisors;
        }
    }
}
