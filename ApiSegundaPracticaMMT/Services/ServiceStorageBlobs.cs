using ApiSegundaPracticaMMT.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Net.Mime;

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
            BlobClient blobClient = containerClient.GetBlobClient(blob);
        
            string contentType = GetContentType(blob);
        
            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            };
        
            await blobClient.UploadAsync(stream, options);
        }
        
        private string GetContentType(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();
        
            if (extension == ".jpg" || extension == ".jpeg")
            {
                return "image/jpeg";
            }
            else if (extension == ".png")
            {
                return "image/png";
            }
            else if (extension == ".gif")
            {
                return "image/gif";
            }
            else if (extension == ".webp")
            {
                return "image/webp";
            }
            else if (extension == ".bmp")
            {
                return "image/bmp";
            }
            else
            {
                return "application/octet-stream";
            }
        }
    }
}
