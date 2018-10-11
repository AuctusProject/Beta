using Auctus.DataAccess.Core;
using Auctus.DataAccess.News;
using Auctus.DataAccessInterfaces.News;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.News
{
    public class NewsSourceData : BaseSql<DomainObjects.News.NewsSource>, INewsSourceData<DomainObjects.News.NewsSource>
    {
        public override string TableName => "NewsSource";

        public NewsSourceData(IConfigurationRoot configuration) : base(configuration) { }
    }
}
