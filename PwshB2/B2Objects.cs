using System;
using System.Collections.Generic;
using Ardalis.SmartEnum;

namespace PwshB2.Api
{
    internal static class Constant
    {
        public const string BaseApiUrl = "https://api.backblazeb2.com/";
        public const string Authorization = "Authorization";
    }
    internal static class B2ApiResouce
    {
        public static readonly Uri BaseUrl = new Uri("https://api.backblazeb2.com/");
        public const string BaseUrlString = "https://api.backblazeb2.com/";
        public const string AuthorizeAccount = "b2api/v1/b2_authorize_account";
        public const string UpdateBucket = "/b2api/v1/b2_update_bucket";
        public const string CreateBucket = "/b2api/v1/b2_create_bucket";
        public const string ListBuckets = "/b2api/v1/b2_list_buckets";
        public const string CancelLargeFile = "/b2api/v1/b2_cancel_large_file";
        public const string DeleteBucket = "/b2api/v1/b2_delete_bucket";
        public const string DeleteFileVersion = "/b2api/v1/b2_delete_file_version";
        public const string DownloadFileById = "/b2api/v1/b2_download_file_by_id";
        public const string DownloadFileByName = "/b2api/v1/b2_download_file_by_name";
        public const string GetDownloadAuthorization = "/b2api/v1/b2_get_download_authorization";
        public const string FinishLargeFile = "/b2api/v1/b2_finish_large_file";
        public const string GetFileInfo = "/b2api/v1/b2_get_file_info";
        public const string GetUploadPartUrl = "/b2api/v1/b2_get_upload_part_url";
        public const string GetUploadUrl = "/b2api/v1/b2_get_upload_url";
        public const string HideFile = "/b2api/v1/b2_hide_file";
        public const string ListFileNames = "/b2api/v1/b2_list_file_names";
        public const string ListFileVersions = "/b2api/v1/b2_list_file_versions";
        public const string ListLargeFileParts = "/b2api/v1/b2_list_parts";
        public const string ListUnfinishedLargeFiles = "/b2api/v1/b2_list_unfinished_large_files";
        public const string StartLargeFile = "/b2api/v1/b2_start_large_file";
        public const string UploadFile = "/b2api/v1/b2_upload_file";
        public const string UploadLargeFilePart = "/b2api/v1/b2_upload_part";
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
