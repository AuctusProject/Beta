using Auctus.DataAccessInterfaces.Advisor;
using Auctus.DomainObjects.Account;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auctus.DomainObjects.Advisor;
using System.Linq;

namespace Auctus.DataAccessMock.Advisor
{
    public class AdvisorData : BaseData<DomainObjects.Advisor.Advisor>, IAdvisorData<DomainObjects.Advisor.Advisor>
    {
        private static List<DomainObjects.Advisor.Advisor> ListAll()
        {
            var advisors = new List<DomainObjects.Advisor.Advisor>();
            advisors.Add(new DomainObjects.Advisor.Advisor()
            {
                Id = 1,
                Email = "test@auctus.org",
                CreationDate = new DateTime(2018, 5, 1, 0, 0, 0),
                ConfirmationDate = new DateTime(2018, 5, 2, 0, 0, 0),
                ConfirmationCode = "",
                Password = "",
                IsAdvisor = true,
                Name = "Tester Advisor",
                Description = "Test Advisor description",
                Enabled = true,
                BecameAdvisorDate = new DateTime(2018, 5, 8, 0, 0, 0),
                UrlGuid = Guid.NewGuid(),
                Wallet = new Wallet()
                {
                    Id = 1,
                    UserId = 1,
                    Address = "0x0000000000000000000000000000000000000000",
                    CreationDate = new DateTime(2018, 5, 3, 0, 0, 0)
                }
            });
            advisors.Add(new DomainObjects.Advisor.Advisor()
            {
                Id = 2,
                Email = "test2@auctus.org",
                CreationDate = new DateTime(2018, 5, 2, 0, 0, 0),
                ConfirmationDate = new DateTime(2018, 5, 3, 0, 0, 0),
                ConfirmationCode = "",
                Password = "",
                IsAdvisor = true,
                Name = "Tester Advisor 2",
                Description = "Test Advisor 2 description",
                Enabled = true,
                BecameAdvisorDate = new DateTime(2018, 5, 9, 0, 0, 0),
                UrlGuid = Guid.NewGuid(),
                Wallet = new Wallet()
                {
                    Id = 2,
                    UserId = 2,
                    Address = "0x0000000000000000000000000000000000000001",
                    CreationDate = new DateTime(2018, 5, 4, 0, 0, 0)
                }
            });
            advisors.Add(new DomainObjects.Advisor.Advisor()
            {
                Id = 3,
                Email = "test3@auctus.org",
                CreationDate = new DateTime(2018, 5, 3, 0, 0, 0),
                ConfirmationDate = new DateTime(2018, 5, 4, 0, 0, 0),
                ConfirmationCode = "",
                Password = "",
                IsAdvisor = true,
                Name = "Tester Advisor 3",
                Description = "Test Advisor 3 description",
                Enabled = true,
                BecameAdvisorDate = new DateTime(2018, 5, 10, 0, 0, 0),
                UrlGuid = Guid.NewGuid(),
                Wallet = new Wallet()
                {
                    Id = 3,
                    UserId = 3,
                    Address = "0x0000000000000000000000000000000000000002",
                    CreationDate = new DateTime(2018, 5, 5, 0, 0, 0)
                }
            });
            return advisors;
        }

        public DomainObjects.Advisor.Advisor GetAdvisor(int id)
        {
            return ListAll().FirstOrDefault(c => c.Id == id);
        }

        public List<DomainObjects.Advisor.Advisor> ListEnabled()
        {
            return ListAll().Where(a => a.Enabled).ToList();
        }

        public IEnumerable<DomainObjects.Advisor.Advisor> ListFollowingAdvisors(int userId)
        {
            var advisorsIds = FollowAdvisorData.FollowAdvisorList.Where(c => c.UserId == userId).Select(c=> c.AdvisorId);
            return ListEnabled().Where(c => advisorsIds.Contains(c.Id));
        }
    }
}
