using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Auctus.Util.NotShared;

namespace Auctus.Util.Azure
{
    //https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet?tabs=windows
    //https://s2.coinmarketcap.com/static/img/coins/32x32/{id}.png
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
                reference.StartCopyAsync(new Uri(url)).Wait();
            }
            else
            {
                throw new OperationCanceledException(string.Format("Could not upload file {0} to blob storage.", fileName));
                    
            }
        }
    }
}
