using Auctus.DataAccessInterfaces.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Storage
{
    public class AzureStorageBusiness
    {
        private const string ASSET_ICON_CONTAINER_NAME = "assetsicons";

        private readonly IAzureStorageResource Resource;

        internal AzureStorageBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider)
        {
            Resource = (IAzureStorageResource)serviceProvider.GetService(typeof(IAzureStorageResource));
        }

        public bool UploadAssetFromUrl(string fileName, string url)
        {
            return Resource.UploadFileFromUrl(ASSET_ICON_CONTAINER_NAME, fileName, url);
        }
    }
}
