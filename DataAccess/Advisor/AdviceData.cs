using Auctus.DataAccess.Core;
using Auctus.DomainObjects.Advisor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Advisor
{
    public class AdviceData : BaseSQL<Advice>
    {
        public override string TableName => "Advice";
    }
}
