using System;
using System.Linq;
using System.Collections.Generic;
using RestSharp;
using RestSharp.Authenticators;
using Ardalis.GuardClauses;

namespace PwshB2.Api
{
    public class B2
    {
        //Run this first, used to get Account object that is used for auth
        public static Account AuthorizeAccount (string userName, string password)
        {
            Guard.Against.NullOrEmpty(userName, nameof(userName));
            Guard.Against.NullOrEmpty(password, nameof(password));
            var client = new RestClient(Constant.BaseApiUrl)
            {
                Authenticator = new HttpBasicAuthenticator(userName, password)
            };
            var req = new RestRequest()
            {
                Method = Method.GET,
                Resource = "b2api/v1/b2_authorize_account",
                RequestFormat = DataFormat.Json
            };
            var responce = client.Execute<Account>(req);
            Guard.Against.RestSharpError(responce);
            Guard.Against.B2Error(responce);
            return responce.Data;
        }

        //This needs to be run in order to set the credentials to use for all B2 actions
        public static void SaveSessionData (Account account) => Session.Instance.accountSession = account;

        //Used to return all bucket associated with an account
        public static List<Bucket> ListBuckets (BucketType type)
        {
            var client = new RestClient()
            {
                BaseUrl = new Uri(Session.Instance.accountSession.apiUrl)
            };
            var req = new RestRequest()
            {
                Method = Method.POST,
                Resource = "/b2api/v1/b2_list_buckets",
                RequestFormat = DataFormat.Json
            };
            var body = new Dictionary<string, string>
            {
                { "accountId", Session.Instance.accountSession.accountId }
            };
            req.AddJsonBody(body);
            req.AddHeader(Constant.Authorization, Session.Instance.accountSession.authorizationToken);
            var responce = client.Execute<Buckets>(req);
            Guard.Against.RestSharpError(responce);
            Guard.Against.B2Error(responce);
            return responce.Data.buckets;
        }

        public static List<Bucket> CreateBucket (List<Bucket> newBucket)
        {
            var buckets = new List<Bucket>();
            foreach (Bucket bucket in newBucket)
            {
                Guard.Against.NullOrEmpty(bucket.bucketName, "bucketName");
                Guard.Against.NullOrEmpty(bucket.bucketType, "bucketType");
                var client = new RestClient()
                {
                    BaseUrl = new Uri(Session.Instance.accountSession.apiUrl)
                };
                var req = new RestRequest()
                {
                    Method = Method.POST,
                    Resource = "/b2api/v1/b2_create_bucket",
                    RequestFormat = DataFormat.Json
                };
                var body = new Dictionary<string, string>
                {
                    { "accountId", Session.Instance.accountSession.accountId },
                    { "bucketName", bucket.bucketName },
                    { "bucketType", bucket.bucketType }
                };
                req.AddJsonBody(body);
                req.AddHeader(Constant.Authorization, Session.Instance.accountSession.authorizationToken);
                var responce = client.Execute<Bucket>(req);
                Guard.Against.RestSharpError(responce);
                Guard.Against.B2Error(responce);
                buckets.Add(responce.Data);
            }
            return buckets;
        }
    }
    //The session data, can be updated but not duplicated
    internal sealed class Session
    {
        public Account accountSession { get; set; }
        private static readonly Lazy<Session> session = new Lazy<Session>(() => new Session());
        public static Session Instance { get { return session.Value; } }
        private Session() { }
    }
}
