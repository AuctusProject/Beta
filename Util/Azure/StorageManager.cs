using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage;

namespace Auctus.Util.Azure
{
    //https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet?tabs=windows
    public static class StorageManager
    {
       public static void UploadFileFromUrl(string storageAccountConfiguration, string containerName, string fileName, string url)
        {
            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(storageAccountConfiguration, out storageAccount))
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
                }
            }
            else
            {
                throw new OperationCanceledException(string.Format("Could not upload file {0} to blob storage.", fileName));
            }
        }
    }
}
