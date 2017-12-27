using System;
using RestSharp;
using Newtonsoft.Json;

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
    }
    internal class Error
    {
        public string code { get; set; }
        public string message { get; set; }
        public int status { get; set; }
    }
}
