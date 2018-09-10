using Auctus.DataAccessInterfaces;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Auctus.DataAccessMock.Account
{
    public class UserData : BaseData<User>, IUserData<User>
    {
        static List<User> users = new List<User>()
        {
            new DomainObjects.Advisor.Advisor()
            {
                Id = 1,
                Email = "test@auctus.org",
                CreationDate = new DateTime(2018, 5, 1, 0, 0, 0),
                ConfirmationDate = new DateTime(2018, 5, 2, 0, 0, 0),
                ConfirmationCode = "",
                Password = "lhs1mr/DnObStvpxSPVZGYNs3muWSgCqHoRlOXoruC8g",//testeauctus
                IsAdvisor = true,
                Name = "Tester Advisor",
                Description = "Test Advisor description",
                Enabled = true,
                BecameAdvisorDate = new DateTime(2018, 5, 8, 0, 0, 0),
                ReferralCode = "0000001",
                UrlGuid = Guid.NewGuid(),
                Wallet = new Wallet()
                {
                    Id = 1,
                    UserId = 1,
                    Address = "0x0000000000000000000000000000000000000000",
                    CreationDate = new DateTime(2018, 5, 3, 0, 0, 0)
                },
                RequestToBeAdvisor = new DomainObjects.Advisor.RequestToBeAdvisor()
                {
                    Id = 1,
                    UserId = 1,
                    Approved = true,
                    CreationDate = new DateTime(2018, 5, 4, 0, 0, 0),
                    Description = "Test Advisor description",
                    Name = "Tester Advisor",
                    PreviousExperience = ""
                }
            },
            new DomainObjects.Account.User()
            {
                Id = 2,
                Email = "test+2@auctus.org",
                CreationDate = new DateTime(2018, 5, 1, 0, 0, 0),
                ConfirmationDate = new DateTime(2018, 5, 2, 0, 0, 0),
                ConfirmationCode = "",
                Password = "lhs1mr/DnObStvpxSPVZGYNs3muWSgCqHoRlOXoruC8g",//testeauctus
                IsAdvisor = false,
                ReferralCode = "0000002",
                Wallet = new Wallet()
                {
                    Id = 2,
                    UserId = 2,
                    Address = "0x0000000000000000000000000000000000000000",
                    CreationDate = new DateTime(2018, 5, 3, 0, 0, 0),
                    AUCBalance = 1000
                }
            },
            new DomainObjects.Advisor.Advisor()
            {
                Id = 3,
                Email = "test+3@auctus.org",
                CreationDate = new DateTime(2018, 5, 1, 0, 0, 0),
                ConfirmationDate = new DateTime(2018, 5, 2, 0, 0, 0),
                ConfirmationCode = "",
                Password = "lhs1mr/DnObStvpxSPVZGYNs3muWSgCqHoRlOXoruC8g",//testeauctus
                IsAdvisor = true,
                Name = "Tester Advisor",
                Description = "Test Advisor description",
                Enabled = true,
                BecameAdvisorDate = new DateTime(2018, 5, 8, 0, 0, 0),
                UrlGuid = Guid.NewGuid(),
                ReferralCode = "0000003",
                ReferredId = 1,
                ReferralStatus = ReferralStatusType.Finished.Value,
                Wallet = new Wallet()
                {
                    Id = 3,
                    UserId = 3,
                    Address = "0x0000000000000000000000000000000000000000",
                    CreationDate = new DateTime(2018, 5, 3, 0, 0, 0)
                },
                RequestToBeAdvisor = new DomainObjects.Advisor.RequestToBeAdvisor()
                {
                    Id = 2,
                    UserId = 3,
                    Approved = true,
                    CreationDate = new DateTime(2018, 5, 4, 0, 0, 0),
                    Description = "Test Advisor description",
                    Name = "Tester Advisor",
                    PreviousExperience = ""
                }
            },
            new DomainObjects.Account.User()
            {
                Id = 4,
                Email = "test+4@auctus.org",
                CreationDate = new DateTime(2018, 5, 1, 0, 0, 0),
                ConfirmationDate = new DateTime(2018, 5, 2, 0, 0, 0),
                ConfirmationCode = "",
                Password = "lhs1mr/DnObStvpxSPVZGYNs3muWSgCqHoRlOXoruC8g",//testeauctus
                IsAdvisor = false,
                ReferralCode = "0000004",
                ReferredId = 1,
                ReferralStatus = ReferralStatusType.InProgress.Value,
                Wallet = new Wallet()
                {
                    Id = 4,
                    UserId = 4,
                    Address = "0x0000000000000000000000000000000000000000",
                    CreationDate = new DateTime(2018, 5, 3, 0, 0, 0),
                    AUCBalance = 1000
                }
            },
            new DomainObjects.Account.User()
            {
                Id = 5,
                Email = "test+5@auctus.org",
                CreationDate = new DateTime(2018, 5, 1, 0, 0, 0),
                ConfirmationDate = new DateTime(2018, 5, 2, 0, 0, 0),
                ConfirmationCode = "",
                Password = "lhs1mr/DnObStvpxSPVZGYNs3muWSgCqHoRlOXoruC8g",//testeauctus
                IsAdvisor = false,
                ReferralCode = "0000005",
                ReferredId = 1,
                ReferralStatus = ReferralStatusType.Interrupted.Value,
                Wallet = new Wallet()
                {
                    Id = 5,
                    UserId = 5,
                    Address = "0x0000000000000000000000000000000000000000",
                    CreationDate = new DateTime(2018, 5, 3, 0, 0, 0),
                    AUCBalance = 1000
                }
            },
            new DomainObjects.Account.User()
            {
                Id = 6,
                Email = "test+6@auctus.org",
                CreationDate = new DateTime(2018, 5, 1, 0, 0, 0),
                ConfirmationDate = new DateTime(2018, 5, 2, 0, 0, 0),
                ConfirmationCode = "",
                Password = "lhs1mr/DnObStvpxSPVZGYNs3muWSgCqHoRlOXoruC8g",//testeauctus
                IsAdvisor = false,
                ReferralCode = "0000006",
                ReferredId = 1,
                ReferralStatus = ReferralStatusType.Paid.Value,
                Wallet = new Wallet()
                {
                    Id = 6,
                    UserId = 6,
                    Address = "0x0000000000000000000000000000000000000000",
                    CreationDate = new DateTime(2018, 5, 3, 0, 0, 0),
                    AUCBalance = 1000
                }
            },
            new DomainObjects.Account.User()
            {
                Id = 7,
                Email = "test+7@auctus.org",
                CreationDate = new DateTime(2018, 5, 1, 0, 0, 0),
                ConfirmationDate = new DateTime(2018, 5, 2, 0, 0, 0),
                ConfirmationCode = "",
                Password = "lhs1mr/DnObStvpxSPVZGYNs3muWSgCqHoRlOXoruC8g",//testeauctus
                IsAdvisor = false,
                ReferralCode = "0000007",
                ReferredId = 1,
                ReferralStatus = ReferralStatusType.Finished.Value,
                Wallet = new Wallet()
                {
                    Id = 7,
                    UserId = 7,
                    Address = "0x0000000000000000000000000000000000000000",
                    CreationDate = new DateTime(2018, 5, 3, 0, 0, 0),
                    AUCBalance = 1000
                }
            }
        };

        public override IEnumerable<User> SelectAll()
        {
            return users;
        }

        public User GetByConfirmationCode(string code)
        {
            return users.FirstOrDefault(c => c.ConfirmationCode == code);
        }

        public User GetByEmail(string email)
        {
            return users.FirstOrDefault(c => c.Email.ToLower() == email.ToLower());
        }

        public User GetById(int id)
        {
            return users.FirstOrDefault(c => c.Id == id);
        }

        public User GetForLogin(string email)
        {
            return GetByEmail(email);
        }

        public User GetForNewWallet(string email)
        {
            return GetByEmail(email);
        }

        public List<User> ListForAucSituation()
        {
            return users.Where(c => c.ReferralStatus == ReferralStatusType.InProgress.Value && c.AllowNotifications).ToList();
        }

        public User GetByReferralCode(string referralCode)
        {
            return users.FirstOrDefault(c => c.ReferralCode == referralCode);
        }

        public List<User> ListReferredUsers(int referredId)
        {
            return users.Where(c => c.ReferredId == referredId).ToList();
        }

        public List<User> ListUsersFollowingAdvisorOrAsset(int advisorId, int assetId)
        {
            return new List<User>();
        }

        public override void Insert(User obj)
        {
            users.Add(obj);
        }

        public override void Update(User obj)
        {
            var user = users.FirstOrDefault(c => c.Id == obj.Id);
            var index = users.IndexOf(user);
            users.RemoveAt(index);
            users.Add(obj);
        }

        public List<User> ListAllUsersData()
        {
            throw new NotImplementedException();
        }

        public User GetForLoginById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
