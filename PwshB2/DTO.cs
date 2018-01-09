using System.Collections.Generic;

namespace PwshB2.Api.Dto
{
    public class DtoAccount
    {
        public string accountId { get; set; }
        public string apiUrl { get; set; }
        public string authorizationToken { get; set; }
        public string downloadUrl { get; set; }
        public long recommnededPartSize { get; set; }
        public long absoluteMinimumPartSize { get; set; }
    }
    public class DtoBucket
    {
        public string accountId { get; set; }
        public string bucketId { get; set; }
        public Dictionary<string, string> bucketInfo { get; set; }
        public string bucketName { get; set; }
        public string bucketType { get; set; }
        public List<DtoCorsRule> corsRules { get; set; }
        public List<DtoLifecycleRule> lifecycleRule { get; set; }
        public long revision { get; set; }
    }
    public class DtoLifecycleRule
    {
        public uint? daysFromHidingToDeleting { get; set; }
        public uint? daysFromUploadingToHiding { get; set; }
        public string fileNamePrefix { get; set; }
    }
    public class DtoCorsRule
    {
        public string corsRuleName { get; set; }
        public List<string> allowedOrigins { get; set; }
        public List<string> allowedOperations { get; set; }
        public List<string> allowedHeaders { get; set; }
        public List<string> exposeHeaders { get; set; }
        public uint maxAgeSeconds { get; set; }
    }
    public class DtoBuckets
    {
        public List<DtoBucket> buckets { get; set; }
    }
    public class DtoFile
    {
        public string fileId { get; set; }
        public string fileName { get; set; }
        public uint contentLength { get; set; }
        public string contentType { get; set; }
        public string contentSha1 { get; set; }
        public Dictionary<string, string> fileInfo { get; set; }
        public string action { get; set; }
        public long uploadTimestamp { get; set; }
    }
    public class DtoFiles
    {
        public List<DtoFile> files { get; set; }
        public string nextFileName { get; set; }
    }
}
