using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessInterfaces.Storage
{
    public interface IAzureStorageResource
    {
        bool UploadFileFromUrl(string containerName, string fileName, string url);
    }
}
