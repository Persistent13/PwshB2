using System;
using RestSharp;
using Newtonsoft.Json;
using PwshB2.Api.Dto;

namespace Ardalis.GuardClauses
{
    public static class B2ErrorGuard
    {
        public static void B2Error(this IGuardClause guardClause, IRestResponse input)
        {
            if ((int)input.StatusCode > 299 || (int)input.StatusCode < 200)
            {
                var error = JsonConvert.DeserializeObject<Error>(input.Content);
                throw new Exception($"Error code '{error.code}'. Error message: {error.message}");
            }
        }
        public static void RestSharpError(this IGuardClause guardClause, IRestResponse input)
        {
            if (!string.IsNullOrEmpty(input.ErrorMessage))
            {
                throw new Exception($"Error: {input.ErrorMessage}");
            }
        }
        public static void InvalidCorsRule(this IGuardClause guardClause, DtoCorsRule corsRule)
        {
            if (corsRule.corsRuleName.StartsWith("b2-"))
            {
                throw new Exception("Names that start with 'b2-' are reserved for Backblaze use.");
            }
            if (corsRule.allowedOrigins.Count == 0)
            {
                throw new Exception("You must supply at least one allowed origin.");
            }
            if (corsRule.allowedOperations.Count == 0)
            {
                throw new Exception("You must supply at least one allowed operation.");
            }
            if (corsRule.allowedHeaders.Contains("*") && corsRule.allowedHeaders.Count > 1)
            {
                throw new Exception("When the '*' entry is specified it must be the only entry.");
            }
            if (corsRule.maxAgeSeconds > 86400)
            {
                throw new Exception("The max age cannot be greater than one day (86,400 seconds).");
            }
        }
        public static void InvalidLifecycleRule(this IGuardClause guardClause, DtoLifecycleRule lifecycleRule)
        {
            if (lifecycleRule.daysFromHidingToDeleting == 0)
            {
                throw new Exception("Setting zero days to delete is not allowed.");
            }
            if (lifecycleRule.daysFromUploadingToHiding == 0)
            {
                throw new Exception("Setting zero days to hide is not allowed.");
            }
            if (lifecycleRule.daysFromUploadingToHiding == null && lifecycleRule.daysFromUploadingToHiding == null)
            {
                throw new Exception("Setting both days to delete and days to hide to null is not allowed.");
            }
        }
    }
    internal class Error
    {
        public string code { get; set; }
        public string message { get; set; }
        public int status { get; set; }
    }
}
