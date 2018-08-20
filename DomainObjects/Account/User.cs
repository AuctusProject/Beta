using Auctus.DomainObjects.Advisor;
using Auctus.Util.DapperAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Account
{
    public class User
    {
        [DapperKey(true)]
        [DapperType(System.Data.DbType.Int32)]
        public int Id { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public string Email { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public string Password { get; set; }
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime CreationDate { get; set; }
        [DapperType(System.Data.DbType.DateTime)]
        public DateTime? ConfirmationDate { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public string ConfirmationCode { get; set; }
        [DapperType(System.Data.DbType.AnsiString)]
        public string ReferralCode { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int? ReferredId { get; set; }
        [DapperType(System.Data.DbType.Int32)]
        public int? ReferralStatus { get; set; }
        [DapperType(System.Data.DbType.Boolean)]
        public bool AllowNotifications { get; set; }

        public bool IsAdvisor { get; set; }
        public Wallet Wallet { get; set; }
        public RequestToBeAdvisor RequestToBeAdvisor { get; set; }
    }
}
