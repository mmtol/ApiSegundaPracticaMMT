using ApiSegundaPracticaMMT.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

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

        public async Task PostBlobAsync(string container, string blob, Stream stream, string contentType)
        {
            BlobContainerClient containerClient = client.GetBlobContainerClient(container);
            BlobClient blobClient = containerClient.GetBlobClient(blob);

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            };

            await blobClient.UploadAsync(stream, options);
        }
    }
}
