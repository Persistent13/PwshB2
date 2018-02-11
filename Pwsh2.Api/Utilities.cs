using System;
using System.Collections.Generic;
using Ardalis.SmartEnum;

namespace PwshB2.Api
{
    internal static class Constant
    {
        public const string Authorization = "Authorization";
        internal class Resouce : SmartEnum<Resouce, int>
        {
            public static Resouce BaseUrlString = new Resouce("https://api.backblazeb2.com/", 0);
            public static Resouce AuthorizeAccount = new Resouce("b2api/v1/b2_authorize_account", 1);
            public static Resouce UpdateBucket = new Resouce("/b2api/v1/b2_update_bucket", 2);
            public static Resouce CreateBucket = new Resouce("/b2api/v1/b2_create_bucket", 3);
            public static Resouce ListBuckets = new Resouce("/b2api/v1/b2_list_buckets", 4);
            public static Resouce CancelLargeFile = new Resouce("/b2api/v1/b2_cancel_large_file", 5);
            public static Resouce DeleteBucket = new Resouce("/b2api/v1/b2_delete_bucket", 6);
            public static Resouce DeleteFileVersion = new Resouce("/b2api/v1/b2_delete_file_version", 7);
            public static Resouce DownloadFileById = new Resouce("/b2api/v1/b2_download_file_by_id", 8);
            public static Resouce DownloadFileByName = new Resouce("/b2api/v1/b2_download_file_by_name", 9);
            public static Resouce GetDownloadAuthorization = new Resouce("/b2api/v1/b2_get_download_authorization", 10);
            public static Resouce FinishLargeFile = new Resouce("/b2api/v1/b2_finish_large_file", 11);
            public static Resouce GetFileInfo = new Resouce("/b2api/v1/b2_get_file_info", 12);
            public static Resouce GetUploadPartUrl = new Resouce("/b2api/v1/b2_get_upload_part_url", 13);
            public static Resouce GetUploadUrl = new Resouce("/b2api/v1/b2_get_upload_url", 14);
            public static Resouce HideFile = new Resouce("/b2api/v1/b2_hide_file", 15);
            public static Resouce ListFileNames = new Resouce("/b2api/v1/b2_list_file_names", 16);
            public static Resouce ListFileVersions = new Resouce("/b2api/v1/b2_list_file_versions", 17);
            public static Resouce ListLargeFileParts = new Resouce("/b2api/v1/b2_list_parts", 18);
            public static Resouce ListUnfinishedLargeFiles = new Resouce("/b2api/v1/b2_list_unfinished_large_files", 19);
            public static Resouce StartLargeFile = new Resouce("/b2api/v1/b2_start_large_file", 20);
            public static Resouce UploadFile = new Resouce("/b2api/v1/b2_upload_file", 21);
            public static Resouce UploadLargeFilePart = new Resouce("/b2api/v1/b2_upload_part", 22);

            protected Resouce(string name, int value) : base(name, value) { }

            public override string ToString() => Name;
            public static implicit operator Resouce(string name) => FromName(name);
            public static implicit operator string(Resouce type) => type.ToString();
            public static implicit operator Resouce(int value) => FromValue(value);
            public static implicit operator int(Resouce type) => Int32.Parse(type);
        }
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
    internal class B2FileParameter : SmartEnum<B2FileParameter, int>
    {
        public static B2FileParameter BucketId = new B2FileParameter("bucketId", 0);
        public static B2FileParameter StartFileName = new B2FileParameter("startFileName", 1);
        public static B2FileParameter MaxFileCount = new B2FileParameter("maxFileCount", 2);
        public static B2FileParameter Prefix = new B2FileParameter("prefix", 3);
        public static B2FileParameter Delimiter = new B2FileParameter("delimiter", 4);

        protected B2FileParameter(string name, int value) : base(name, value) { }

        public override string ToString() => Name;
        public static implicit operator B2FileParameter(string name) => FromName(name);
        public static implicit operator string(B2FileParameter type) => type.ToString();
        public static implicit operator B2FileParameter(int value) => FromValue(value);
        public static implicit operator int(B2FileParameter type) => Int32.Parse(type);
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
