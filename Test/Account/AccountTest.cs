using Auctus.Model;
using Auctus.Util.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Auctus.Test.Account
{
    public class AccountTest : BaseTest
    {
        [Theory]
        [InlineData("testenewuser@auctus.org", "testeauctus")]
        public void RegisterUser(string email, string password)
        {
            Task.WaitAll(UserBusiness.RegisterAsync(email, password, null));
            var user = UserBusiness.GetByEmail(email);
            Assert.NotNull(user);
            Assert.Equal(email, user.Email);
        }

        [Fact]
        public void ConfirmEmail()
        {
            LoggedEmail = "ConfirmEmail@auctus.org";
            Task.WaitAll(UserBusiness.RegisterAsync(LoggedEmail, "testeauctus", null));
            var user = UserBusiness.GetByEmail(LoggedEmail);
            var loginResponse = UserBusiness.ConfirmEmail(user.ConfirmationCode);
            Assert.NotNull(loginResponse);
            Assert.False(loginResponse.PendingConfirmation);
        }

        [Theory]
        [InlineData("test@auctus.org", "testeauctus")]
        public void Login(string email, string password)
        {
            var loginResponse = UserBusiness.Login(email, password);
            Assert.NotNull(loginResponse);
            Assert.Equal(email, loginResponse.Email);
        }

        [Fact]
        public void GetReferralInfo()
        {
            var referralInfo = UserBusiness.GetReferralProgramInfo();
            Assert.NotNull(referralInfo);
            Assert.Equal(2, referralInfo.FinishedCount);
            Assert.Equal(1, referralInfo.InProgressCount);
            Assert.Equal(1, referralInfo.InterruptedCount);
            Assert.Equal(0, referralInfo.NotStartedCount);
            Assert.Equal(1, referralInfo.PaidCount);
            Assert.Equal("0000001", referralInfo.ReferralCode);
        }

        [Fact]
        public void RegisterWithReferral()
        {
            Task.WaitAll(UserBusiness.RegisterAsync("RegisterWithReferral@auctus.org", "testeauctus", "0000001"));

            var referralInfo = UserBusiness.GetReferralProgramInfo();

            Assert.NotNull(referralInfo);
            Assert.Equal(2, referralInfo.FinishedCount);
            Assert.Equal(1, referralInfo.InProgressCount);
            Assert.Equal(1, referralInfo.InterruptedCount);
            Assert.Equal(1, referralInfo.NotStartedCount);
            Assert.Equal(1, referralInfo.PaidCount);

            Assert.Equal("0000001", referralInfo.ReferralCode);
        }

        [Fact]
        public void SetReferral()
        {
            LoggedEmail = "SetReferral@auctus.org";
            
            Task.WaitAll(UserBusiness.RegisterAsync(LoggedEmail, "testeauctus", null));

            Assert.Throws<BusinessException>(() => UserBusiness.SetReferralCode("INVALID_CODE"));

            UserBusiness.SetReferralCode("0000001");
            var user = UserBusiness.GetByEmail(LoggedEmail);
            Assert.Equal(1, user.ReferredId);
        }
    }
}

