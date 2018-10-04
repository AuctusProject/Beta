using Auctus.DataAccessInterfaces.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.Business.Storage
{
    public class AzureStorageBusiness
    {
        private const string ASSET_ICON_CONTAINER_NAME = "assetsicons";
        private const string USER_PICTURE_CONTAINER_NAME = "userpicture";
        private const string AGENCIES_CONTAINER_NAME = "agencieslogo";
        private const string REPORT_CONTAINER_NAME = "assetsreport";

        private readonly IAzureStorageResource Resource;

        internal AzureStorageBusiness(IConfigurationRoot configuration, IServiceProvider serviceProvider)
        {
            Resource = (IAzureStorageResource)serviceProvider.GetService(typeof(IAzureStorageResource));
        }

        public async Task<bool> UploadAssetFromUrlAsync(string fileName, string url)
        {
            return await Resource.UploadFileFromUrlAsync(ASSET_ICON_CONTAINER_NAME, fileName, url);
        }

        public async Task<bool> UploadUserPictureFromBytesAsync(string fileName, byte[] file)
        {
            return await Resource.UploadFileFromBytesAsync(USER_PICTURE_CONTAINER_NAME, fileName, file);
        }

        public async Task<bool> DeleteUserPicture(string fileName)
        {
            return await Resource.DeleteFileAsync(USER_PICTURE_CONTAINER_NAME, fileName);
        }

        public async Task<bool> UploadAgenciesFromByteAsync(string fileName, byte[] file)
        {
            return await Resource.UploadFileFromBytesAsync(AGENCIES_CONTAINER_NAME, fileName, file);
        }

        public async Task<bool> UploadAssetReportFromByteAsync(string fileName, byte[] file)
        {
            return await Resource.UploadFileFromBytesAsync(REPORT_CONTAINER_NAME, fileName, file, "application/pdf");
        }
    }
}
