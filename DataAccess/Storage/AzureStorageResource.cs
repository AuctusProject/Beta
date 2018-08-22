using Auctus.DataAccessInterfaces.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccess.Storage
{
    //https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet?tabs=windows
    public class AzureStorageResource : IAzureStorageResource
    {
        private readonly string StorageConfiguration;

        public AzureStorageResource(IConfigurationRoot configuration)
        {
            StorageConfiguration = configuration.GetSection("ConnectionString:Storage").Get<string>();
        }

        public bool UploadFileFromUrl(string containerName, string fileName, string url)
        {
            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(StorageConfiguration, out storageAccount))
            {
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(containerName);
                container.CreateIfNotExistsAsync().Wait();
                var reference = container.GetBlockBlobReference(fileName);
                try
                {
                    reference.StartCopyAsync(new Uri(url)).Wait();
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                throw new OperationCanceledException(string.Format("Could not upload file {0} to blob storage.", fileName));
            }
            return true;
        }
    }
}
