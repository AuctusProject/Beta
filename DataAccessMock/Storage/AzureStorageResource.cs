using Auctus.DataAccessInterfaces.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DataAccessMock.Storage
{
    public class AzureStorageResource : IAzureStorageResource
    {
        public bool UploadFileFromUrl(string containerName, string fileName, string url)
        {
            throw new NotImplementedException();
        }
    }
}
