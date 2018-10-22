using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using Dapper;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Account
{
    public class ActionData : BaseSql<DomainObjects.Account.Action>, IActionData<DomainObjects.Account.Action>
    {
        public override string TableName => "Action";
        public ActionData(IConfigurationRoot configuration) : base(configuration) { }

        private const string SQL_LIST = @"SELECT a.* 
                                            FROM 
                                            [Action] a WITH(NOLOCK)
                                            WHERE 
                                            a.CreationDate >= @CreationDate {0}";

        public List<DomainObjects.Account.Action> FilterActivity(DateTime startDate, params ActionType[] actionTypes)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("CreationDate", startDate, DbType.DateTime);
            var complement = "";
            if (actionTypes?.Any() == true)
            {
                complement = $" AND ({string.Join(" OR ", actionTypes.Select((c, i) => $"a.Type = @Type{i}"))})";
                for(var i = 0; i < actionTypes.Length; ++i)
                    parameters.Add($"Type{i}", actionTypes.ElementAt(i), DbType.Int32);
            }
            return Query<DomainObjects.Account.Action>(string.Format(SQL_LIST, complement), parameters).ToList();
        }

        //public List<DomainObjects.Account.Action> FilterActivity(DateTime startDate, params ActionType[] actionTypes)
        //{
        //    var filterBuilder = Builders<DomainObjects.Account.Action>.Filter;
        //    var filter = filterBuilder.Gte(c => c.CreationDate, startDate);
        //    if (actionTypes?.Any() == true)
        //        filter = filter & filterBuilder.In(c => c.Type, actionTypes.Select(a => a.Value));
        //    return Collection.Find(filter).ToList();
        //}
    }
}
