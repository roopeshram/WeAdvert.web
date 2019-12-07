using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Amazon.S3;
using Amazon.S3.Model;




namespace WebAdvert.Web.Services
{
    public class S3FileUploader : IFileUploader
    {
        private readonly IConfiguration _configuration;

        public S3FileUploader(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> UploadFileAsync(string filename, Stream storagestream)
        {
            if (string.IsNullOrEmpty(filename)) throw new ArgumentException("filename must be specified");

            var bucketname = _configuration.GetValue<string>("ImageBucket");
            using (var client = new AmazonS3Client())
            {
                if (storagestream.Length > 0)
                    if (storagestream.CanSeek)
                        storagestream.Seek(0, SeekOrigin.Begin);

                var request = new PutObjectRequest
                {
                    AutoCloseStream = true,
                    BucketName = bucketname,
                    InputStream = storagestream,
                    Key = filename
                };
                var response = await client.PutObjectAsync(request);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
        }
    }
}
