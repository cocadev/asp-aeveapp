using System;
using System.IO;
using System.Threading.Tasks;
using AevApp.Helper.Interface;
using AevApp.Service.Interface;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace AevApp.Service.Implementation
{
    public class StorageService : IStorageService
    {
        private readonly string _storageAccountName;
        private readonly string _storageAccountKey;
        private readonly string _blobContainer;

        public StorageService(IConfigManager configManager)
        {
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }

            _storageAccountName = configManager.Get("StorageAccountName");
            _storageAccountKey = configManager.Get("StorageAccountKey");
            _blobContainer = configManager.Get("BlobContainerName");
        }

        public async Task UploadImageAsync(Stream fileStream, string fileName, string accountName = null, string accountKey = null, string blobContainer = null)
        {
            var storageCredentials = new StorageCredentials(accountName?? _storageAccountName, accountKey ?? _storageAccountKey);
            var storageAccount = new CloudStorageAccount(storageCredentials, true);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(blobContainer ?? _blobContainer);
            var blockedBlob = container.GetBlockBlobReference(fileName);
            await blockedBlob.UploadFromStreamAsync(fileStream);
        }
    }
}