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
        public B2(string userName, string password) => AuthorizeAccount(userName, password);
        public B2(Account session) => SaveSessionData(session);

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
        private void AuthorizeAccount (string userName, string password)
        {
            Guard.Against.NullOrEmpty(userName, nameof(userName));
            Guard.Against.NullOrEmpty(password, nameof(password));
            var client = new RestClient(Constant.Resouce.BaseUrlString)
            {
                Authenticator = new HttpBasicAuthenticator(userName, password)
            };
            var req = new RestRequest()
            {
                Method = Method.GET,
                Resource = Constant.Resouce.AuthorizeAccount,
                RequestFormat = DataFormat.Json
            };
            var responce = client.Execute<Account>(req);
            Guard.Against.RestSharpError(responce);
            Guard.Against.B2Error(responce);
            SaveSessionData(responce.Data);
        }

        /// <summary>
        /// This needs to be run in order to set the credentials to use for all B2 actions
        /// </summary>
        /// <param name="account">The account object returned by <c>AuthorizeAccount</c></param>
        private void SaveSessionData (Account account) => Session.Instance.accountSession = account;

        /// <summary>
        /// Used to return all buckets associated with an account.
        /// </summary>
        /// <param name="type">The bucket type to return.</param>
        /// <returns>A list of buckets, if any are present.</returns>
        public List<Bucket> ListBuckets (BucketType type)
        {
            var body = new Dictionary<string, string>
            {
                { "accountId", Session.Instance.accountSession.AccountId }
            };
            return ExecuteB2Request<DtoBuckets>(body, Constant.Resouce.ListBuckets).Buckets;
        }

        /// <summary>
        /// Creates a new bucket.
        /// </summary>
        /// <param name="name">The new name of the bucket. A name must be alphanumeric, at least six characters, and a max of fifty characters.</param>
        /// <param name="type">Only public and private types are valid when creating a bucket.</param>
        /// <returns>The updated bucket.</returns>
        public Bucket CreateBucket (string name, BucketType type)
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
            return ExecuteB2Request<Bucket>(body, Constant.Resouce.CreateBucket);
        }

        /// <summary>
        /// Change the bucket to private or public.
        /// </summary>
        /// <param name="name">The new name of the bucket. A name must be alphanumeric, at least six characters, and a max of fifty characters.</param>
        /// <param name="type">Only public and private types are valid when changing a bucket's type.</param>
        public void SetBucketType (string name, BucketType type)
        {
            var body = new Dictionary<string, string>
            {
                { "accountId", Session.Instance.accountSession.AccountId },
                { "bucketType", HashMap.FromBucketType[type] },
                { "bucketId", GetBucketIdFromName(name) }
            };
            var responce = ExecuteB2Request<Bucket>(body, Constant.Resouce.UpdateBucket);
            if (responce.BucketType != HashMap.FromBucketType[type])
            {
                throw new B2Exception($"Bucket {name} failed to update.");
            }
        }

        /// <summary>
        /// A helper method to run B2 REST requests.
        /// </summary>
        /// <typeparam name="T">The IDto interface object to return.</typeparam>
        /// <param name="body">An all string Dictionary object the represents the body of a request. Unique per REST resource.</param>
        /// <param name="resource">A B2Resource enumerable type that represents a B2 REST resource.</param>
        /// <returns>An object based on the IDto interface.</returns>
        private static T ExecuteB2Request<T> (Dictionary<string, string> body, Constant.Resouce resource)
            where T : IDto, new()
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

        /// <summary>
        /// Remove a bucket. The bucket cannot be restored.
        /// </summary>
        /// <param name="name">The new name of the bucket. A name must be alphanumeric, at least six characters, and a max of fifty characters.</param>
        public void RemoveBucket (string name)
        {
            var body = new Dictionary<string, string>
            {
                { "accountId", Session.Instance.accountSession.AccountId },
                { "bucketId", GetBucketIdFromName(name) }
            };
            ExecuteB2Request<Bucket>(body, Constant.Resouce.DeleteBucket);
        }

        public void SetCorsRules ()
        {
            throw new NotImplementedException();
        }

        public void SetLifecycleRules ()
        {
            throw new NotImplementedException();
        }

        public List<File> ListFiles(string name, string filterPrefix)
        {
            DtoFiles returnData;
            var files = new List<File>();
            var body = new Dictionary<string, string>
            {
                { "", "" }
            };
            string bucketId = GetBucketIdFromName(name);
            if(!string.IsNullOrEmpty(filterPrefix))
            {
                body.Add(B2FileParameter.Prefix, filterPrefix);
            }
            do
            {
                returnData = ExecuteB2Request<DtoFiles>(body, Constant.Resouce.ListFileNames);
                body.Remove("nextFileName");
                body.Add("nextFileName", returnData.NextFileName);
            } while (!string.IsNullOrEmpty(returnData.NextFileName));
            return files;
        }

        internal static string GetBucketIdFromName (string bucketName)
        {
            try
            {
                return new B2(Session.Instance.accountSession).ListBuckets(BucketType.All).Find(bucket => bucket.BucketName == bucketName).BucketId;
            }
            catch (NullReferenceException err)
            {
                throw new B2ObjectNotFound($"Unable to find the bucket: {bucketName}", err);
            }
        }
    }
    //The session data, can be updated but not duplicated
    public sealed class Session
    {
        public Account accountSession { get; set; }
        private static readonly Lazy<Session> session = new Lazy<Session>(() => new Session());
        public static Session Instance { get { return session.Value; } }
        private Session() { }
    }
}
