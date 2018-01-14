using System;
using System.Linq;
using System.Collections.Generic;
using RestSharp;
using RestSharp.Authenticators;
using Ardalis.GuardClauses;
using PwshB2.Api.Dto;
using PwshB2.Exceptions;

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
                Resource = B2Resouce.AuthorizeAccount,
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
            var body = new Dictionary<string, string>
            {
                { "accountId", Session.Instance.accountSession.AccountId }
            };
            return ExecuteB2Request<DtoBuckets>(body, B2Resouce.ListBuckets).buckets;
        }

        public static Bucket CreateBucket (string name, BucketType type)
        {
            Guard.Against.NullOrEmpty(name, nameof(name));
            var body = new Dictionary<string, string>
            {
                { "accountId", Session.Instance.accountSession.AccountId },
                { "bucketName", name },
                { "bucketType", HashMap.FromBucketType[type] }
            };
            return ExecuteB2Request<Bucket>(body, B2Resouce.CreateBucket);
        }

        private static IEnumerable<Bucket> UpdateBucket (List<Bucket> bucketsToUpdate)
        {
            if (bucketsToUpdate.Count == 0) { throw new B2InvalidCountException(""); }
            foreach (var bucket in bucketsToUpdate)
            {
                var client = new RestClient()
                {
                    BaseUrl = Session.Instance.accountSession.ApiUrl
                };
                var req = new RestRequest()
                {
                    Method = Method.POST,
                    Resource = B2Resouce.UpdateBucket,
                    RequestFormat = DataFormat.Json
                };
                var body = new Dictionary<string, string>
                {
                    { "accountId", Session.Instance.accountSession.AccountId }
                };
                if (!string.IsNullOrEmpty(bucket.BucketName)) { body.Add("bucketName", bucket.BucketName); }
                if (!string.IsNullOrEmpty(bucket.BucketType)) { body.Add("bucketType", bucket.BucketType); }
                if (!string.IsNullOrEmpty(bucket.BucketId)) { body.Add("bucketId", bucket.BucketId); }
                req.AddJsonBody(body);
                req.AddHeader(Constant.Authorization, Session.Instance.accountSession.AuthorizationToken);
                var responce = client.Execute<Bucket>(req);
                Guard.Against.RestSharpError(responce);
                Guard.Against.B2Error(responce);
                yield return responce.Data;
            }
        }

        public static void SetBucketType (string name, BucketType type)
        {
            var body = new Dictionary<string, string>
            {
                { "accountId", Session.Instance.accountSession.AccountId },
                { "bucketType", HashMap.FromBucketType[type] },
                { "bucketId", GetBucketIdFromName(name) }
            };
            var responce = ExecuteB2Request<Bucket>(body, B2Resouce.UpdateBucket);
            if (responce.BucketType != HashMap.FromBucketType[type])
            {
                throw new B2Exception($"Bucket {name} failed to update.");
            }
        }

        private static T ExecuteB2Request<T> (Dictionary<string, string> body, B2Resouce resource)
            where T : new()
        {
            Guard.Against.Null(body, nameof(body));
            Guard.Against.EmptyDictionary(body, nameof(body));
            var req = new RestRequest()
            {
                Method = Method.POST,
                Resource = resource,
                RequestFormat = DataFormat.Json
            };
            var client = new RestClient()
            {
                BaseUrl = Session.Instance.accountSession.ApiUrl
            };
            req.AddBody(body);
            req.AddHeader(Constant.Authorization, Session.Instance.accountSession.AuthorizationToken);
            var resp = client.Execute<T>(req);
            Guard.Against.RestSharpError(resp);
            Guard.Against.B2Error(resp);
            return resp.Data;
        }

        public static List<Bucket> RenameBucket (List<Bucket> buckets)
        {
            var updatedBuckets = new List<Bucket>();
            throw new NotImplementedException();
        }
        //public static List<DtoBucket> RenameBucket ()
        //{

        //}

        public static void SetCorsRules ()
        {
            throw new NotImplementedException();
        }

        public static void SetLifecycleRules ()
        {
            throw new NotImplementedException();
        }

        public static void DeleteBucket (List<Bucket> bucketsToDelete)
        {
            var buckets = new List<Bucket>();
            foreach (var bucket in bucketsToDelete)
            {
                throw new NotImplementedException();
            }
        }

        internal static string GetBucketIdFromName (string bucketName)
        {
            try
            {
                return ListBuckets(BucketType.All).Find(bucket => bucket.BucketName == bucketName).BucketId;
            }
            catch (NullReferenceException err)
            {
                throw new B2ObjectNotFound($"Unable to find the bucket: {bucketName}", err);
            }
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
