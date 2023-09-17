using Amazon.S3.Model;

namespace AWS.web.Controllers
{
    internal class putObjectTaggingRequest
    {
        public putObjectTaggingRequest()
        {
        }

        public string bucketName { get; set; }
        public string Key { get; set; }
        public Tagging Tagging { get; set; }
    }
}