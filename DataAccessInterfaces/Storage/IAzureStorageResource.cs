using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.DataAccessInterfaces.Storage
{
    public interface IAzureStorageResource
    {
        Task<bool> UploadFileFromUrlAsync(string containerName, string fileName, string url);
        Task<bool> UploadFileFromBytesAsync(string containerName, string fileName, byte[] file, string contentType = null);
        Task<bool> DeleteFileAsync(string containerName, string fileName);
    }
}
