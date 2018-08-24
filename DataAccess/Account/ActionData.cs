using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Account;
using Auctus.DomainObjects.Account;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Account
{
    public class ActionData : BaseMongo<DomainObjects.Account.Action>, IActionData<DomainObjects.Account.Action>
    {
        public override string CollectionName => "Action";
        public ActionData(IConfigurationRoot configuration) : base(configuration) { }

        public List<DomainObjects.Account.Action> FilterActivity(DateTime startDate, params ActionType[] actionTypes)
        {
            var filterBuilder = Builders<DomainObjects.Account.Action>.Filter;
            var filter = filterBuilder.Gte(c => c.CreationDate, startDate);
            if (actionTypes?.Any() == true)
                filter = filter & filterBuilder.In(c => c.Type, actionTypes.Select(a => a.Value));
            return Collection.Find(filter).ToList();
        }
    }
}
