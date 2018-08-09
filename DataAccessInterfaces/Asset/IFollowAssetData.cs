﻿using Auctus.DataAccessInterfaces;
using Auctus.DomainObjects.Asset;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessInterfaces.Asset
{
    public interface IFollowAssetData<T> : IBaseData<T>
    {
        List<FollowAsset> ListFollowers(IEnumerable<int> assetsIds);
    }
}