using Auctus.DataAccess.Core;
using Auctus.DomainObjects.Account;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Account
{
    public class WalletData : BaseSQL<Wallet>
    {
        public override string TableName => "Wallet";

        private const string SQL_BY_USER = @"SELECT w.* FROM [Wallet] w WHERE w.UserId = @UserId AND w.CreationDate = (SELECT MAX(w2.CreationDate) FROM [Wallet] w2 WHERE w2.UserId = w.UserId";

        public Wallet GetByUser(int userId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("UserId", userId, DbType.Int32);
            return Query<Wallet>(SQL_BY_USER, parameters).SingleOrDefault();
        }
    }
}
