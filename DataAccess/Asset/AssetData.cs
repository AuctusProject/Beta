using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Auctus.DataAccess.Asset
{
    public class AssetData : BaseSQL<Auctus.DomainObjects.Asset.Asset>
    {
        public override string TableName => "Asset";
    }
}
