using System;
using System.Collections.Generic;
using Ardalis.SmartEnum;

namespace PwshB2.Api
{
    public class Account
    {
        public string accountId { get; set; }
        public string apiUrl { get; set; }
        public string authorizationToken { get; set; }
        public string downloadUrl { get; set; }
        public long recommnededPartSize { get; set; }
        public long absoluteMinimumPartSize { get; set; }
    }
    public class Bucket
    {
        public string accountId { get; set; }
        public string bucketId { get; set; }
        public Dictionary<string, string> bucketInfo { get; set; }
        public string bucketName { get; set; }
        public BucketEnum bucketType { get; set; }
        public List<Dictionary<string, string>> corsRules { get; set; }
        public List<Dictionary<string, string>> lifecycleRule { get; set; }
        public long revision { get; set; }
    }
    public class Buckets
    {
        public List<Bucket> buckets { get; set; }
    }
    internal struct Constant
    {
        public const string BaseApiUrl = "https://api.backblazeb2.com/";
        public const string AuthorizeAccountResource = "b2api/v1/b2_authorize_account";
        public const string Authorization = "Authorization";
    }
    public enum BucketEnum
    {
        all = 0,
        allPublic = 1,
        allPrivate = 2,
        snapshot = 3
    }
    public class BucketType : SmartEnum<BucketType, int>
    {
        public static BucketType All = new BucketType("all", 0);
        public static BucketType Public = new BucketType("allPublic", 1);
        public static BucketType Private = new BucketType("allPrivate", 2);
        public static BucketType Snapshot = new BucketType("snapshot", 3);

        protected BucketType(string name, int value) : base(name, value) { }

        public override string ToString() => Name;
        public static implicit operator BucketType(string name) => FromName(name);
        public static implicit operator string(BucketType type) => type.ToString();
        public static implicit operator BucketType(int value) => FromValue(value);
        public static implicit operator int(BucketType type) => Int32.Parse(type);
    }
}
