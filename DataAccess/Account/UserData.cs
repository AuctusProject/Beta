using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using Auctus.DomainObjects.Advisor;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Account
{
    public class UserData : BaseSQL<User>, IUserData<User>
    {
        public override string TableName => "User";

        private const string SQL_FOR_LOGIN = @"SELECT u.*, a.*, r.*, w.* 
                                                FROM 
                                                [User] u
                                                LEFT JOIN [Advisor] a ON a.Id = u.Id
                                                LEFT JOIN [RequestToBeAdvisor] r ON r.UserId = u.Id AND r.CreationDate = (SELECT MAX(r2.CreationDate) FROM [RequestToBeAdvisor] r2 WHERE r2.UserId = u.Id)
                                                LEFT JOIN [Wallet] w ON w.UserId = u.Id AND w.CreationDate = (SELECT MAX(w2.CreationDate) FROM [Wallet] w2 WHERE w2.UserId = u.Id)
                                                WHERE 
                                                u.Email = @Email";

        private const string SQL_FOR_CONFIRMATION = @"SELECT u.*, r.*
                                                FROM 
                                                [User] u
                                                LEFT JOIN [RequestToBeAdvisor] r ON r.UserId = u.Id AND r.CreationDate = (SELECT MAX(r2.CreationDate) FROM [RequestToBeAdvisor] r2 WHERE r2.UserId = u.Id)
                                                WHERE 
                                                u.ConfirmationCode = @Code";

        private const string SQL_FOR_NEW_WALLET = @"SELECT u.*, a.*, r.*
                                                FROM 
                                                [User] u
                                                LEFT JOIN [Advisor] a ON a.Id = u.Id
                                                LEFT JOIN [RequestToBeAdvisor] r ON r.UserId = u.Id AND r.CreationDate = (SELECT MAX(r2.CreationDate) FROM [RequestToBeAdvisor] r2 WHERE r2.UserId = u.Id)
                                                WHERE 
                                                u.Email = @Email";

        private const string SQL_BY_EMAIL = @"SELECT u.*, a.*
                                                FROM 
                                                [User] u
                                                LEFT JOIN [Advisor] a ON a.Id = u.Id
                                                WHERE 
                                                u.Email = @Email";

        private const string SQL_BY_ID = @"SELECT u.*, a.*
                                                FROM 
                                                [User] u
                                                LEFT JOIN [Advisor] a ON a.Id = u.Id
                                                WHERE 
                                                u.Id = @Id";

        public User GetForLogin(string email)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Email", email.ToLower().Trim(), DbType.AnsiString);
            return Query<User, Auctus.DomainObjects.Advisor.Advisor, RequestToBeAdvisor, Wallet, User>(SQL_FOR_LOGIN,
                        (user, advisor, request, wallet) =>
                        {
                            if (advisor != null)
                            {
                                advisor.Id = user.Id;
                                advisor.Email = user.Email;
                                advisor.Password = user.Password;
                                advisor.CreationDate = user.CreationDate;
                                advisor.ConfirmationCode = user.ConfirmationCode;
                                advisor.ConfirmationDate = user.ConfirmationDate;
                                advisor.IsAdvisor = true;
                                advisor.Wallet = wallet;
                                advisor.RequestToBeAdvisor = request;
                                return advisor;
                            }
                            else
                            {
                                user.Wallet = wallet;
                                user.RequestToBeAdvisor = request;
                                return user;
                            }
                        }, "Id,Id,Id", parameters).SingleOrDefault();
        }

        public User GetByConfirmationCode(string code)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Code", code, DbType.AnsiString);
            return Query<User, RequestToBeAdvisor, User>(SQL_FOR_CONFIRMATION,
                        (user, request) =>
                        {
                            user.RequestToBeAdvisor = request;
                            return user;
                        }, "Id", parameters).SingleOrDefault();
        }

        public User GetForNewWallet(string email)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Email", email.ToLower().Trim(), DbType.AnsiString);
            return Query<User, Auctus.DomainObjects.Advisor.Advisor, RequestToBeAdvisor, User>(SQL_FOR_NEW_WALLET,
                        (user, advisor, request) =>
                        {
                            if (advisor != null)
                            {
                                advisor.Id = user.Id;
                                advisor.Email = user.Email;
                                advisor.Password = user.Password;
                                advisor.CreationDate = user.CreationDate;
                                advisor.ConfirmationCode = user.ConfirmationCode;
                                advisor.ConfirmationDate = user.ConfirmationDate;
                                advisor.IsAdvisor = true;
                                advisor.RequestToBeAdvisor = request;
                                return advisor;
                            }
                            else
                            {
                                user.RequestToBeAdvisor = request;
                                return user;
                            }
                        }, "Id,Id", parameters).SingleOrDefault();
        }

        public User GetByEmail(string email)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Email", email.ToLower().Trim(), DbType.AnsiString);
            return Query<User, Auctus.DomainObjects.Advisor.Advisor, User>(SQL_BY_EMAIL,
                        (user, advisor) =>
                        {
                            if (advisor != null)
                            {
                                advisor.Id = user.Id;
                                advisor.Email = user.Email;
                                advisor.Password = user.Password;
                                advisor.CreationDate = user.CreationDate;
                                advisor.ConfirmationCode = user.ConfirmationCode;
                                advisor.ConfirmationDate = user.ConfirmationDate;
                                advisor.IsAdvisor = true;
                                return advisor;
                            }
                            else
                                return user;
                        }, "Id", parameters).SingleOrDefault();
        }

        public User GetById(int id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);
            return Query<User, Auctus.DomainObjects.Advisor.Advisor, User>(SQL_BY_ID,
                        (user, advisor) =>
                        {
                            if (advisor != null)
                            {
                                advisor.Id = user.Id;
                                advisor.Email = user.Email;
                                advisor.Password = user.Password;
                                advisor.CreationDate = user.CreationDate;
                                advisor.ConfirmationCode = user.ConfirmationCode;
                                advisor.ConfirmationDate = user.ConfirmationDate;
                                advisor.IsAdvisor = true;
                                return advisor;
                            }
                            else
                                return user;
                        }, "Id", parameters).SingleOrDefault();
        }
    }
}
