using Auctus.DomainObjects.Advisor;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Auctus.DataAccess.Advisor
{
    public class AdvisorData : BaseSQL<DomainObjects.Advisor.Advisor>
    {
        public override string TableName => "Advisor";
    }
}
