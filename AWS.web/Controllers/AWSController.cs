using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AWS.web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AWSController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly object _client;

        public AWSController(IConfiguration config)
        {
            _config = config;  
        }
        [HttpGet("list-buckets")]
        public async Task<IActionResult> ListBuckets() {
			var awsAccess = _config.GetValue<string>("ÄWSSDK:AccessKey");
			var awsSecret = _config.GetValue<string>("ÄWSSDK:SecretKey");

            var client = new AmazonS3Client(awsAccess, awsSecret, RegionEndpoint.EUNorth1);

            try
            {
                var result = await client.ListBucketsAsync();
                return Ok(result);
            }
            catch (Exception)
            {

				return BadRequest("Buckets couldnot be listed");
			}
        }
        [HttpPost("create-bucket/{name}")]
        public async Task<IActionResult> CreateBucket(string name)
        {
            var awsAccess = _config.GetValue<string>("ÄWSSDK:AccessKey");
            var awsSecret = _config.GetValue<string>("ÄWSSDK:SecretKey");

            var client = new AmazonS3Client(awsAccess, awsSecret, RegionEndpoint.EUNorth1);

            try
            {
                PutBucketRequest request = new PutBucketRequest() { BucketName = name };
               
               await client.PutBucketAsync(request);
                return Ok($"Bucket: {name} WAS created");
            }
            catch (Exception)
            {

                return BadRequest($"Bucket: {name} WAS NOT created");
            }
        }

        [HttpPost("create-object/{bucketName}/{objectName}")]
        public async Task<IActionResult> CreateObject(string bucketName, string objectName)
        {
            var awsAccess = _config.GetValue<string>("ÄWSSDK:AccessKey");
            var awsSecret = _config.GetValue<string>("ÄWSSDK:SecretKey");

            var client = new AmazonS3Client(awsAccess, awsSecret, RegionEndpoint.EUNorth1);

            try
            {
                PutObjectRequest request = new PutObjectRequest() 
                { 
                    BucketName = bucketName,
                    Key = objectName,
                    ContentType = "text/plain",
                    ContentBody = "Welcome to Linkedin learning"
                };

                await client.PutObjectAsync(request);

                ListObjectsRequest objectsRequest = new ListObjectsRequest()
                {
                    BucketName = bucketName
                };
                ListObjectsResponse response = await client.ListObjectsAsync(objectsRequest);


                return Ok(response);
            }
            catch (Exception)
            {

                return BadRequest($"File created/uploaded");
            }
        }

        [HttpGet("list-objects/{bucketName}")]
        public async Task<IActionResult> ListObjects(string bucketName)
        {
            var awsAccess = _config.GetValue<string>("ÄWSSDK:AccessKey");
            var awsSecret = _config.GetValue<string>("ÄWSSDK:SecretKey");

            var client = new AmazonS3Client(awsAccess, awsSecret, RegionEndpoint.EUNorth1);

            try
            {

                ListObjectsRequest objectsRequest = new ListObjectsRequest()
                {
                    BucketName = bucketName
                };
                ListObjectsResponse response = await client.ListObjectsAsync(objectsRequest);


                return Ok(response);
            }
            catch (Exception)
            {

                return BadRequest($"File created/uploaded");
            }
        }

        [HttpPost("create-folder/{bucketName}/{folderName}")]
        public async Task<IActionResult> CreateFolder(string bucketName, string folderName)
        {
            var awsAccess = _config.GetValue<string>("ÄWSSDK:AccessKey");
            var awsSecret = _config.GetValue<string>("ÄWSSDK:SecretKey");

            var client = new AmazonS3Client(awsAccess, awsSecret, RegionEndpoint.EUNorth1);

            try
            {
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = bucketName,
                    Key = folderName.Replace("%2F", "/")
                };

                await client.PutObjectAsync(request);
                return Ok($" {folderName} folder was created inside {bucketName}");
            }
            catch (Exception)
            {

                return BadRequest("The folder COULD NOT be created ");
            }
        }

        [HttpDelete("delete-bucket/{bucketName}")]
        public async Task<IActionResult> DeleteBucket(string bucketName)
        {
            var awsAccess = _config.GetValue<string>("ÄWSSDK:AccessKey");
            var awsSecret = _config.GetValue<string>("ÄWSSDK:SecretKey");

            var client = new AmazonS3Client(awsAccess, awsSecret, RegionEndpoint.EUNorth1);

            try
            {
                DeleteBucketRequest request = new DeleteBucketRequest()
                {
                    BucketName = bucketName
                };

                await client.DeleteBucketAsync(request);
                return Ok($" {bucketName}  was deleted ");
            }
            catch (Exception)
            {

                return BadRequest($"{bucketName} WAS NOT deleted  ");
            }
        }

        [HttpDelete("delete-bucket-object/{bucketName}/{objectName}")]
        public async Task<IActionResult> DeleteBucketObject(string bucketName, string objectName)
        {
            var awsAccess = _config.GetValue<string>("ÄWSSDK:AccessKey");
            var awsSecret = _config.GetValue<string>("ÄWSSDK:SecretKey");

            var client = new AmazonS3Client(awsAccess, awsSecret, RegionEndpoint.EUNorth1);

            try
            {
                DeleteObjectRequest request = new DeleteObjectRequest()
                {
                    BucketName = bucketName,
                    Key = objectName
                };

                await client.DeleteObjectAsync(request);
                return Ok($" {objectName}in {bucketName}  was deleted ");
            }
            catch (Exception)
            {

                return BadRequest($"{objectName} in {bucketName} WAS NOT deleted  ");
            }
        }

        [HttpDelete("cleanup-bucket/{bucketName}")]
        public async Task<IActionResult> CleanupBucket(string bucketName)
        {
            var awsAccess = _config.GetValue<string>("ÄWSSDK:AccessKey");
            var awsSecret = _config.GetValue<string>("ÄWSSDK:SecretKey");

            var client = new AmazonS3Client(awsAccess, awsSecret, RegionEndpoint.EUNorth1);

            try
            {
                DeleteObjectsRequest request = new DeleteObjectsRequest()
                {
                    BucketName = bucketName,
                    Objects = new List<KeyVersion>
                    {
                        new KeyVersion(){Key = "token key.txt"}
                    }
                };

                await client.DeleteObjectsAsync(request);
                return Ok($" {bucketName}  was cleaned up ");
            }
            catch (Exception)
            {

                return BadRequest($" {bucketName} WAS NOT cleaned up  ");
            }
        }

        [HttpPost("enable-versioning/{bucketName}")]
        public async Task<IActionResult> EnableVersioning(string bucketName)
        {
            var awsAccess = _config.GetValue<string>("ÄWSSDK:AccessKey");
            var awsSecret = _config.GetValue<string>("ÄWSSDK:SecretKey");

            var client = new AmazonS3Client(awsAccess, awsSecret, RegionEndpoint.EUNorth1);

            try
            {
                PutBucketVersioningRequest request = new PutBucketVersioningRequest
                { 
                    BucketName = bucketName, 
                    VersioningConfig = new S3BucketVersioningConfig
                    {
                        Status = VersionStatus.Enabled
                    }
                };

                await client.PutBucketVersioningAsync(request);

                return Ok($"Bucket: {bucketName} versioning ENABLED!");
            }
            catch (Exception)
            {

                return BadRequest($"Bucket: {bucketName} versioning NOT ENABLED! ");
            }
        }

        [HttpPut("add-metadata/{bucketName}/{fileName}")]
        public async Task<IActionResult> AddMetadata(string bucketName, string fileName)
        {
            var awsAccess = _config.GetValue<string>("ÄWSSDK:AccessKey");
            var awsSecret = _config.GetValue<string>("ÄWSSDK:SecretKey");

            var client = new AmazonS3Client(awsAccess, awsSecret, RegionEndpoint.EUNorth1);

            try
            {
                Tagging newTags = new Tagging()
                {
                    TagSet = new List<Tag>
                    {
                        new Tag {Key = "Key1",Value = "FirstTag"},
                        new Tag {Key = "Key2",Value = "SecondTag"}
                    }
                };
                PutObjectTaggingRequest request = new PutObjectTaggingRequest()
                {
                    BucketName = bucketName,
                    Key = fileName,
                    Tagging = newTags
                };
                await client.PutObjectTaggingAsync(request);

                return Ok($"Tags Added");
            }
            catch (Exception)
            {

                return BadRequest($"Tags COULD NOT be added ");
            }
        }

        [HttpPut("copy-file/{sourceBucket}/{sourceKey}/{destinationBucket}/{destinationKey}")]
        public async Task<IActionResult> CopyFile(string sourceBucket, string sourceKey, string destinationBucket, string destinationKey)
        {
            var awsAccess = _config.GetValue<string>("ÄWSSDK:AccessKey");
            var awsSecret = _config.GetValue<string>("ÄWSSDK:SecretKey");

            var client = new AmazonS3Client(awsAccess, awsSecret, RegionEndpoint.EUNorth1);

            try
            {
                CopyObjectRequest request = new CopyObjectRequest()
                {
                    SourceBucket = sourceBucket,
                    SourceKey = sourceKey,
                    DestinationBucket = destinationBucket,
                    DestinationKey = destinationKey
                };
                await client.CopyObjectAsync(request);

                return Ok($"Object/File was copied");
            }
            catch (Exception)
            {

                return BadRequest($"Object/File WAS NOT copied ");
            }
        }

        [HttpPut("generate-download-link/{bucketName}/{keyName}")]
        public async Task<IActionResult> GenerateDownloadLink(string bucketName, string keyName)
        {
            var awsAccess = _config.GetValue<string>("ÄWSSDK:AccessKey");
            var awsSecret = _config.GetValue<string>("ÄWSSDK:SecretKey");

            var client = new AmazonS3Client(awsAccess, awsSecret, RegionEndpoint.EUNorth1);

            try
            {
                GetPreSignedUrlRequest request = new GetPreSignedUrlRequest()
                {
                    BucketName = bucketName,
                    Key = keyName,
                    Expires = DateTime.Now.AddHours(5),
                    Protocol = Protocol.HTTP
                };
                string downloadLink = client.GetPreSignedURL(request);

                return Ok($"DownloadLink - {downloadLink}");
            }
            catch (Exception)
            {

                return BadRequest("Download link was NOT generated ");
            }
        }
    }
}
