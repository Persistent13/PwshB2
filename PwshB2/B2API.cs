using System;
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
        /// <summary>
        /// Used to get authorization to the B2 service as well as various other API minutia.
        /// </summary>
        /// <remarks>
        /// Authorization will expire after some time, rerun this method to re-establish authorization.
        /// </remarks>
        /// <param name="userName">The AccountId for the B2 account.</param>
        /// <param name="password">The ApplicationKey for the B2 account.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Throw when the userName or password parameters are empty or null.</exception>
        /// <exception cref="B2HttpException">Thrown when there is an HTTP level issue communicating with the B2 service.</exception>
        /// <exception cref="B2Exception">Thrown when a B2 service issue occurs.</exception>
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

        /// <summary>
        /// This needs to be run in order to set the credentials to use for all B2 actions
        /// </summary>
        /// <param name="account">The account object returned by <c>AuthorizeAccount</c></param>
        public static void SaveSessionData (Account account) => Session.Instance.accountSession = account;

        /// <summary>
        /// Used to return all buckets associated with an account.
        /// </summary>
        /// <param name="type">The bucket type to return.</param>
        /// <returns></returns>
        public static List<Bucket> ListBuckets (BucketType type)
        {
            var body = new Dictionary<string, string>
            {
                { "accountId", Session.Instance.accountSession.AccountId }
            };
            return ExecuteB2Request<DtoBuckets>(body, B2Resouce.ListBuckets).buckets;
        }

        /// <summary>
        /// Creates a new bucket.
        /// </summary>
        /// <param name="name">The new name of the bucket. A name must be alphanumeric, at least six characters, and a max of fifty characters.</param>
        /// <param name="type">Only public and private types are valid when creating a bucket.</param>
        /// <returns></returns>
        public static Bucket CreateBucket (string name, BucketType type)
        {
            Guard.Against.B2NameError(name);
            Guard.Against.B2BucketTypeError(type);
            Guard.Against.NullOrEmpty(name, nameof(name));
            var body = new Dictionary<string, string>
            {
                { "accountId", Session.Instance.accountSession.AccountId },
                { "bucketName", name },
                { "bucketType", HashMap.FromBucketType[type] }
            };
            return ExecuteB2Request<Bucket>(body, B2Resouce.CreateBucket);
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

        public static void RemoveBucket (string name)
        {
            var body = new Dictionary<string, string>
            {
                { "accountId", Session.Instance.accountSession.AccountId },
                { "bucketId", GetBucketIdFromName(name) }
            };
            ExecuteB2Request<Bucket>(body, B2Resouce.DeleteBucket);
        }

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
            throw new NotImplementedException();
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
