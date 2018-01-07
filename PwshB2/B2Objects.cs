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
        public string bucketType { get; set; }
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
        public const string Authorization = "Authorization";
    }
    public class HashMap
    {
        public static Dictionary<string, BucketType> ToBucketType = new Dictionary<string, BucketType>
        {
            { "all", BucketType.All },
            { "allPublic", BucketType.Public },
            { "allPrivate", BucketType.Private },
            { "snapshot", BucketType.Snapshot }
        };
        public static Dictionary<BucketType, string> FromBucketType = new Dictionary<BucketType, string>
        {
            { BucketType.All, "all" },
            { BucketType.Public, "allPublic" },
            { BucketType.Private, "allPrivate" },
            { BucketType.Snapshot, "snapshot" }
        };
    }
    public class BucketType : SmartEnum<BucketType, int>
    {
        public static BucketType All = new BucketType("All", 0);
        public static BucketType Public = new BucketType("Public", 1);
        public static BucketType Private = new BucketType("Private", 2);
        public static BucketType Snapshot = new BucketType("Snapshot", 3);

        protected BucketType(string name, int value) : base(name, value) { }

        public override string ToString() => Name;
        public static implicit operator BucketType(string name) => FromName(name);
        public static implicit operator string(BucketType type) => type.ToString();
        public static implicit operator BucketType(int value) => FromValue(value);
        public static implicit operator int(BucketType type) => Int32.Parse(type);
    }
}
