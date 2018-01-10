using RestSharp;
using Newtonsoft.Json;
using PwshB2.Api.Dto;
using PwshB2.Exceptions;

namespace Ardalis.GuardClauses
{
    public static class B2ErrorGuard
    {
        public static void B2Error(this IGuardClause guardClause, IRestResponse input)
        {
            if ((int)input.StatusCode > 299 || (int)input.StatusCode < 200)
            {
                var error = JsonConvert.DeserializeObject<DtoError>(input.Content);
                throw new B2Exception($"Error code '{error.code}'. Error message: {error.message}");
            }
        }
        public static void RestSharpError(this IGuardClause guardClause, IRestResponse input)
        {
            if (!string.IsNullOrEmpty(input.ErrorMessage))
            {
                throw new B2HttpException($"Error: {input.ErrorMessage}");
            }
        }
        public static void InvalidCorsRule(this IGuardClause guardClause, DtoCorsRule corsRule)
        {
            if (corsRule.corsRuleName.StartsWith("b2-"))
            {
                throw new B2NameException("Names that start with 'b2-' are reserved for Backblaze use.");
            }
            if (corsRule.allowedOrigins.Count == 0)
            {
                throw new B2InvalidCountException("You must supply at least one allowed origin.");
            }
            if (corsRule.allowedOperations.Count == 0)
            {
                throw new B2InvalidCountException("You must supply at least one allowed operation.");
            }
            if (corsRule.allowedHeaders.Contains("*") && corsRule.allowedHeaders.Count > 1)
            {
                throw new B2InvalidHeaderException("When the '*' entry is specified it must be the only entry.");
            }
            if (corsRule.maxAgeSeconds > 86400)
            {
                throw new B2InvalidCountException("The max age cannot be greater than one day (86,400 seconds).");
            }
        }
        public static void InvalidLifecycleRule(this IGuardClause guardClause, DtoLifecycleRule lifecycleRule)
        {
            if (lifecycleRule.daysFromHidingToDeleting == 0)
            {
                throw new B2InvalidLifecycleRuleException("Setting zero days to delete files is not allowed.");
            }
            if (lifecycleRule.daysFromUploadingToHiding == 0)
            {
                throw new B2InvalidLifecycleRuleException("Setting zero days to hide files is not allowed.");
            }
            if (lifecycleRule.daysFromUploadingToHiding == null && lifecycleRule.daysFromUploadingToHiding == null)
            {
                throw new B2InvalidLifecycleRuleException("Setting both days to delete and days to hide to null is not allowed.");
            }
        }
        public static void InvalidBucket(this IGuardClause guardClause, DtoBucket bucket)
        {
            if (bucket.bucketName.Length < 6 || bucket.bucketName.Length > 50)
            {
                throw new B2NameException("A bucket's name cannot be less than 6 or more than 50 characters.");
            }
            if (bucket.bucketName.StartsWith("b2-"))
            {
                throw new B2NameException("Names that start with 'b2-' are reserved for Backblaze use.");
            }
            if (bucket.bucketInfo.Count > 10)
            {
                throw new B2InvalidCountException("You cannot have more than 10 bucket info entries.");
            }
            foreach (string key in bucket.bucketInfo.Keys)
            {
                if (key.Length > 25)
                {
                    throw new B2InvalidCountException("You cannot have a string greater than 25 characters in the bucket info.");
                }
            }
            foreach (string value in bucket.bucketInfo.Values)
            {
                if (value.Length > 25)
                {
                    throw new B2InvalidCountException("You cannot have a string greater than 25 characters in the bucket info.");
                }
            }
        }
        public static void InvalidFile(this IGuardClause guardClause, DtoFile file)
        {
            if (file.fileName.StartsWith("/"))
            {
                throw new B2NameException("File names cannot start with '/'.");
            }
            if (file.fileName.EndsWith("/"))
            {
                throw new B2NameException("File names cannot end with '/'.");
            }
            if (file.fileName.Contains("//"))
            {
                throw new B2NameException("File names cannot contain '//'.");
            }
            if (file.fileName.Contains("\\"))
            {
                throw new B2NameException("File names cannot contain '\'.");
            }
            foreach (string section in file.fileName.Split('/'))
            {
                if (section.Length > 125)
                {
                    throw new B2NameException("A segment or name has exceeded 125 characters. A segment is the part between each '/'.");
                }
            }
            if (file.fileInfo.Count > 10)
            {
                throw new B2InvalidCountException("You cannot have more than 10 file info entries.");
            }
            foreach (string key in file.fileInfo.Keys)
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
            foreach (string value in file.fileInfo.Values)
            {
                if (value.Length > 25)
                {
                    throw new B2InvalidCountException("You cannot have a string greater than 25 characters in the file info.");
                }
            }
        }
    }
}
