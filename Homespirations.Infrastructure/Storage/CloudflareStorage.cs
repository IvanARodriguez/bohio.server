using System.Net;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Homespirations.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Homespirations.Infrastructure.Storage
{
    public class CloudStorage : ICloudStorage
    {
        private readonly AmazonS3Client _s3Client;
        private static readonly string _bucketName = "homespace-media";
        private static readonly string _accessKey = Environment.GetEnvironmentVariable("CLOUDFLARE_ACCESS_KEY") ?? "";
        private static readonly string _secretKey = Environment.GetEnvironmentVariable("CLOUDFLARE_SECRET_KEY") ?? "";
        private static readonly string _account = Environment.GetEnvironmentVariable("CLOUDFLARE_ACCOUNT") ?? "";
        private readonly ILogger<CloudStorage> _logger;

        public CloudStorage(ILogger<CloudStorage> logger)
        {
            var credentials = new BasicAWSCredentials(_accessKey, _secretKey);

            _s3Client = new AmazonS3Client(credentials, new AmazonS3Config
            {
                ServiceURL = $"https://{_account}.r2.cloudflarestorage.com",
                ForcePathStyle = true,
                RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
                ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED,
            });

            _logger = logger;
        }

        /// <summary>
        /// Generates a pre-signed URL for uploading files directly to Cloudflare R2.
        /// </summary>
        public string GetPreSignedUploadUrl(string fileName, string contentType)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                Verb = HttpVerb.PUT,
                ContentType = contentType,
                Expires = DateTime.UtcNow.AddMinutes(30)
            };

            return _s3Client.GetPreSignedURL(request);
        }

        /// <summary>
        /// Returns the public URL of an uploaded file.
        /// </summary>
        public string? GetPublicFileUrl(string fileName)
        {
            // TODO: Change to domain name
            // https://{domain}/{_bucketName}/{fileName}

            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            _logger.LogDebug("Public file URL: https://pub-352d598dded441409abdb6bbdf9852d8.r2.dev/{FILENAME}", fileName);

            return $"https://pub-352d598dded441409abdb6bbdf9852d8.r2.dev/{fileName}";
        }

        /// <summary>
        /// Directly uploads a file to Cloudflare R2.
        /// </summary>
        public async Task<string> UploadAsync(byte[] imageData, string fileName, string contentType)
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                InputStream = new MemoryStream(imageData),
                ContentType = contentType,
                AutoCloseStream = true,
                DisablePayloadSigning = true, // Cloudflare R2 requires this
                UseChunkEncoding = false      // This is the fix
            };

            var response = await _s3Client.PutObjectAsync(putRequest);

            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Failed to upload to Cloudflare. Status code: {response.HttpStatusCode}");
            }

            return GetPublicFileUrl(fileName) ?? throw new Exception("Failed to get public file URL."); ;
        }

    }
}
