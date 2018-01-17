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
    internal class B2Resouce : SmartEnum<B2Resouce, int>
    {
        public static B2Resouce BaseUrlString = new B2Resouce("https://api.backblazeb2.com/", 0);
        public static B2Resouce AuthorizeAccount = new B2Resouce("b2api/v1/b2_authorize_account", 1);
        public static B2Resouce UpdateBucket = new B2Resouce("/b2api/v1/b2_update_bucket", 2);
        public static B2Resouce CreateBucket = new B2Resouce("/b2api/v1/b2_create_bucket", 3);
        public static B2Resouce ListBuckets = new B2Resouce("/b2api/v1/b2_list_buckets", 4);
        public static B2Resouce CancelLargeFile = new B2Resouce("/b2api/v1/b2_cancel_large_file", 5);
        public static B2Resouce DeleteBucket = new B2Resouce("/b2api/v1/b2_delete_bucket", 6);
        public static B2Resouce DeleteFileVersion = new B2Resouce("/b2api/v1/b2_delete_file_version", 7);
        public static B2Resouce DownloadFileById = new B2Resouce("/b2api/v1/b2_download_file_by_id", 8);
        public static B2Resouce DownloadFileByName = new B2Resouce("/b2api/v1/b2_download_file_by_name", 9);
        public static B2Resouce GetDownloadAuthorization = new B2Resouce("/b2api/v1/b2_get_download_authorization", 10);
        public static B2Resouce FinishLargeFile = new B2Resouce("/b2api/v1/b2_finish_large_file", 11);
        public static B2Resouce GetFileInfo = new B2Resouce("/b2api/v1/b2_get_file_info", 12);
        public static B2Resouce GetUploadPartUrl = new B2Resouce("/b2api/v1/b2_get_upload_part_url", 13);
        public static B2Resouce GetUploadUrl = new B2Resouce("/b2api/v1/b2_get_upload_url", 14);
        public static B2Resouce HideFile = new B2Resouce("/b2api/v1/b2_hide_file", 15);
        public static B2Resouce ListFileNames = new B2Resouce("/b2api/v1/b2_list_file_names", 16);
        public static B2Resouce ListFileVersions = new B2Resouce("/b2api/v1/b2_list_file_versions", 17);
        public static B2Resouce ListLargeFileParts = new B2Resouce("/b2api/v1/b2_list_parts", 18);
        public static B2Resouce ListUnfinishedLargeFiles = new B2Resouce("/b2api/v1/b2_list_unfinished_large_files", 19);
        public static B2Resouce StartLargeFile = new B2Resouce("/b2api/v1/b2_start_large_file", 20);
        public static B2Resouce UploadFile = new B2Resouce("/b2api/v1/b2_upload_file", 21);
        public static B2Resouce UploadLargeFilePart = new B2Resouce("/b2api/v1/b2_upload_part", 22);

        protected B2Resouce(string name, int value) : base(name, value) { }

        public override string ToString() => Name;
        public static implicit operator B2Resouce(string name) => FromName(name);
        public static implicit operator string(B2Resouce type) => type.ToString();
        public static implicit operator B2Resouce(int value) => FromValue(value);
        public static implicit operator int(B2Resouce type) => Int32.Parse(type);
    }
}
