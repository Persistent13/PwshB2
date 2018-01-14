using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PwshB2.Api.Dto
{
    public class Account
    {
        public string AccountId { get; set; }
        [JsonConverter(typeof(string))]
        public Uri ApiUrl { get; set; }
        public string AuthorizationToken { get; set; }
        [JsonConverter(typeof(string))]
        public Uri DownloadUrl { get; set; }
        public long RecommendedPartSize { get; set; }
        public long AbsoluteMinimumPartSize { get; set; }
    }
    public class Bucket
    {
        public string AccountId { get; set; }
        public string BucketId { get; set; }
        public Dictionary<string, string> BucketInfo { get; set; }
        public string BucketName { get; set; }
        public string BucketType { get; set; }
        public List<CorsRule> CorsRules { get; set; }
        public List<LifecycleRule> LifecycleRule { get; set; }
        public long Revision { get; set; }
    }
    public class LifecycleRule
    {
        public uint? DaysFromHidingToDeleting { get; set; }
        public uint? DaysFromUploadingToHiding { get; set; }
        public string FileNamePrefix { get; set; }
    }
    public class CorsRule
    {
        public string CorsRuleName { get; set; }
        public List<string> AllowedOrigins { get; set; }
        public List<string> AllowedOperations { get; set; }
        public List<string> AllowedHeaders { get; set; }
        public List<string> ExposeHeaders { get; set; }
        public uint MaxAgeSeconds { get; set; }
    }
    public class DtoBuckets
    {
        public List<Bucket> buckets { get; set; }
    }
    public class File
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public uint ContentLength { get; set; }
        public string ContentType { get; set; }
        public string ContentSha1 { get; set; }
        public Dictionary<string, string> FileInfo { get; set; }
        public string Action { get; set; }
        public long UploadTimestamp { get; set; }
    }
    public class DtoFiles
    {
        public List<File> Files { get; set; }
        public string NextFileName { get; set; }
    }
    public class Error
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
    }
}
