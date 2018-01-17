using RestSharp;
using Newtonsoft.Json;
using PwshB2.Api;
using PwshB2.Api.Dto;
using PwshB2.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace Ardalis.GuardClauses
{
    public static class B2ErrorGuard
    {
        public static void B2Error(this IGuardClause guardClause, IRestResponse input)
        {
            if ((int)input.StatusCode == 429)
            {
                var error = JsonConvert.DeserializeObject<Error>(input.Content);
                throw new B2Exception($"You have been rate limited, reduce the amount of requests run. B2 Message: {error.Message}");
            }
            if ((int)input.StatusCode == 500)
            {
                var error = JsonConvert.DeserializeObject<Error>(input.Content);
                throw new B2Exception($"The B2 service is temporarily unavailable. B2 Message: {error.Message}");
            }
            if ((int)input.StatusCode > 299 || (int)input.StatusCode < 200)
            {
                var error = JsonConvert.DeserializeObject<Error>(input.Content);
                throw new B2Exception($"Error code '{error.Code}'. Error message: {error.Message}");
            }
        }
        public static void B2NameError(this IGuardClause guardClause, string name)
        {
            if (name.Length < 6)
            {
                throw new B2NameException("A bucket name must be longer than five characters.");
            }
            if (name.Length > 50)
            {
                throw new B2NameException("A bucket name must be shorter than fifty-one characters.");
            }
            if (!name.All(c => IsValidBucketCharacters(c)))
            {
                throw new B2NameException("A bucket name must consist of alphanumeric characters only.");
            }
        }
        private static bool IsValidBucketCharacters (char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        public static void B2BucketTypeError(this IGuardClause guardClause, BucketType type)
        {
            if (type != BucketType.Public || type != BucketType.Private)
            {
                throw new B2BucketTypeError("A bucket can only be private or public in this context.");
            }
        }
        public static void RestSharpError(this IGuardClause guardClause, IRestResponse input)
        {
            if (!string.IsNullOrEmpty(input.ErrorMessage))
            {
                throw new B2HttpException($"Error: {input.ErrorMessage}");
            }
        }
        public static void EmptyDictionary(this IGuardClause guardClause, Dictionary<string, string> dictionary, string parameterName)
        {
            if (dictionary.Count == 0)
            {
                throw new B2InvalidCountException($"The parameter {parameterName} cannot be empty.");
            }
        }
        public static void InvalidCorsRule(this IGuardClause guardClause, CorsRule corsRule)
        {
            if (corsRule.CorsRuleName.StartsWith("b2-"))
            {
                throw new B2NameException("Names that start with 'b2-' are reserved for Backblaze use.");
            }
            if (corsRule.AllowedOrigins.Count == 0)
            {
                throw new B2InvalidCountException("You must supply at least one allowed origin.");
            }
            if (corsRule.AllowedOperations.Count == 0)
            {
                throw new B2InvalidCountException("You must supply at least one allowed operation.");
            }
            if (corsRule.AllowedHeaders.Contains("*") && corsRule.AllowedHeaders.Count > 1)
            {
                throw new B2InvalidHeaderException("When the '*' entry is specified it must be the only entry.");
            }
            if (corsRule.MaxAgeSeconds > 86400)
            {
                throw new B2InvalidCountException("The max age cannot be greater than one day (86,400 seconds).");
            }
        }
        public static void InvalidLifecycleRule(this IGuardClause guardClause, LifecycleRule lifecycleRule)
        {
            if (lifecycleRule.DaysFromHidingToDeleting == 0)
            {
                throw new B2InvalidLifecycleRuleException("Setting zero days to delete files is not allowed.");
            }
            if (lifecycleRule.DaysFromUploadingToHiding == 0)
            {
                throw new B2InvalidLifecycleRuleException("Setting zero days to hide files is not allowed.");
            }
            if (lifecycleRule.DaysFromUploadingToHiding == null && lifecycleRule.DaysFromUploadingToHiding == null)
            {
                throw new B2InvalidLifecycleRuleException("Setting both days to delete and days to hide to null is not allowed.");
            }
        }
        public static void InvalidBucket(this IGuardClause guardClause, Bucket bucket)
        {
            if (bucket.BucketName.Length < 6 || bucket.BucketName.Length > 50)
            {
                throw new B2NameException("A bucket's name cannot be less than 6 or more than 50 characters.");
            }
            if (bucket.BucketName.StartsWith("b2-"))
            {
                throw new B2NameException("Names that start with 'b2-' are reserved for Backblaze use.");
            }
            if (bucket.BucketInfo.Count > 10)
            {
                throw new B2InvalidCountException("You cannot have more than 10 bucket info entries.");
            }
            foreach (string key in bucket.BucketInfo.Keys)
            {
                if (key.Length > 25)
                {
                    throw new B2InvalidCountException("You cannot have a string greater than 25 characters in the bucket info.");
                }
            }
            foreach (string value in bucket.BucketInfo.Values)
            {
                if (value.Length > 25)
                {
                    throw new B2InvalidCountException("You cannot have a string greater than 25 characters in the bucket info.");
                }
            }
        }
        public static void InvalidFile(this IGuardClause guardClause, File file)
        {
            if (file.FileName.StartsWith("/"))
            {
                throw new B2NameException("File names cannot start with '/'.");
            }
            if (file.FileName.EndsWith("/"))
            {
                throw new B2NameException("File names cannot end with '/'.");
            }
            if (file.FileName.Contains("//"))
            {
                throw new B2NameException("File names cannot contain '//'.");
            }
            if (file.FileName.Contains("\\"))
            {
                throw new B2NameException("File names cannot contain '\'.");
            }
            foreach (string section in file.FileName.Split('/'))
            {
                if (section.Length > 125)
                {
                    throw new B2NameException("A segment or name has exceeded 125 characters. A segment is the part between each '/'.");
                }
            }
            if (file.FileInfo.Count > 10)
            {
                throw new B2InvalidCountException("You cannot have more than 10 file info entries.");
            }
            foreach (string key in file.FileInfo.Keys)
            {
                if (key.Length > 25)
                {
                    throw new B2InvalidCountException("You cannot have a string greater than 25 characters in the file info.");
                }
                if (key.StartsWith("b2-"))
                {
                    if (key == "b2-content-disposition") { continue; }
                    throw new B2NameException("Names that start with 'b2-' are reserved for Backblaze use.");
                }
            }
            foreach (string value in file.FileInfo.Values)
            {
                if (value.Length > 25)
                {
                    throw new B2InvalidCountException("You cannot have a string greater than 25 characters in the file info.");
                }
            }
        }
    }
}
