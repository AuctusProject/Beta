using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Account
{
    public class WalletData : BaseSQL<Wallet>, IWalletData<Wallet>
    {
        public override string TableName => "Wallet";

        private const string SQL_BY_USER = @"SELECT w.* FROM [Wallet] w WHERE w.UserId = @UserId AND w.CreationDate = (SELECT MAX(w2.CreationDate) FROM [Wallet] w2 WHERE w2.UserId = w.UserId";

        private const string SQL_BY_ADDRESS = @"SELECT w.* 
                                                FROM 
                                                [Wallet] w 
                                                INNER JOIN (SELECT wa.UserId, MAX(wa.CreationDate) AS CreationDate FROM [Wallet] wa GROUP BY wa.UserId) b
                                                            ON w.UserId = b.UserId AND w.CreationDate = b.CreationDate
                                                WHERE 
                                                w.Address = @Address";


        public Wallet GetByUser(int userId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("UserId", userId, DbType.Int32);
            return Query<Wallet>(SQL_BY_USER, parameters).SingleOrDefault();
        }

        public Wallet GetByAddress(string address)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Address", address, DbType.AnsiStringFixedLength);
            return Query<Wallet>(SQL_BY_ADDRESS, parameters).SingleOrDefault();
        }
    }
}
