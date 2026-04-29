using ApiSegundaPracticaMMT.Models;
using Azure.Storage.Blobs;

namespace ApiSegundaPracticaMMT.Services
{
    public class ServiceStorageBlobs
    {
        private BlobServiceClient client;

        public ServiceStorageBlobs(BlobServiceClient client)
        {
            this.client = client;
        }

        public async Task<ModelStorage> FindBlobAsync(string container, string blob)
        {
            BlobContainerClient containerClient = client.GetBlobContainerClient(container);
            BlobClient blobClient = containerClient.GetBlobClient(blob);

            ModelStorage model = new ModelStorage
            {
                Blob = blob,
                Container = container,
                Uri = blobClient.Uri.AbsoluteUri
            };

            return model;
        }

        public async Task PostBlobAsync(string container, string blob, Stream stream)
        {
            BlobContainerClient containerClient = client.GetBlobContainerClient(container);
            await containerClient.UploadBlobAsync(blob, stream);
        }
    }
}
