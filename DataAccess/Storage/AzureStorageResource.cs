using Auctus.DataAccess.Core;
using Auctus.DataAccessInterfaces.Storage;
using Auctus.Util.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<bool> UploadFileFromUrlAsync(string containerName, string fileName, string url)
        {
            var reference = await GetBlockBlobReferenceAsync(containerName, fileName);
            try
            {
                await reference.StartCopyAsync(new Uri(url));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UploadFileFromBytesAsync(string containerName, string fileName, byte[] file)
        {
            var reference = await GetBlockBlobReferenceAsync(containerName, fileName);
            try
            {
                await reference.UploadFromByteArrayAsync(file, 0, file.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteFileAsync(string containerName, string fileName)
        {
            var reference = await GetBlockBlobReferenceAsync(containerName, fileName);
            try
            {
                await reference.DeleteIfExistsAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<Microsoft.WindowsAzure.Storage.Blob.CloudBlockBlob> GetBlockBlobReferenceAsync(string containerName, string fileName)
        {
            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(StorageConfiguration, out storageAccount))
            {
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(containerName);
                await container.CreateIfNotExistsAsync();
                return container.GetBlockBlobReference(fileName);
            }
            else
                throw new OperationCanceledException($"Could not upload the file {fileName} to container {containerName} on blob storage. Configuration problem.");
        }
    }
}
