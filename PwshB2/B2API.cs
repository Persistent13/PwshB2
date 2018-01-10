﻿using System;
using System.Linq;
using System.Collections.Generic;
using RestSharp;
using RestSharp.Authenticators;
using Ardalis.GuardClauses;
using PwshB2.Api.Dto;

namespace PwshB2.Api
{
    public class B2
    {
        //Run this first, used to get Account object that is used for auth
        public static DtoAccount AuthorizeAccount (string userName, string password)
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
                Resource = B2ApiResouce.AuthorizeAccount,
                RequestFormat = DataFormat.Json
            };
            var responce = client.Execute<DtoAccount>(req);
            Guard.Against.RestSharpError(responce);
            Guard.Against.B2Error(responce);
            return responce.Data;
        }

        //This needs to be run in order to set the credentials to use for all B2 actions
        public static void SaveSessionData (DtoAccount account) => Session.Instance.accountSession = account;

        //Used to return all bucket associated with an account
        public static List<DtoBucket> ListBuckets (BucketType type)
        {
            var client = new RestClient()
            {
                BaseUrl = new Uri(Session.Instance.accountSession.apiUrl)
            };
            var req = new RestRequest()
            {
                Method = Method.POST,
                Resource = B2ApiResouce.ListBuckets,
                RequestFormat = DataFormat.Json
            };
            var body = new Dictionary<string, string>
            {
                { "accountId", Session.Instance.accountSession.accountId }//,
                //{ "bucketTypes", HashMap.FromBucketType[type] }
            };
            req.AddJsonBody(body);
            req.AddHeader(Constant.Authorization, Session.Instance.accountSession.authorizationToken);
            var responce = client.Execute<DtoBuckets>(req);
            Guard.Against.RestSharpError(responce);
            Guard.Against.B2Error(responce);
            return responce.Data.buckets;
        }

        public static List<DtoBucket> CreateBucket (List<DtoBucket> newBucket)
        {
            var buckets = new List<DtoBucket>();
            foreach (var bucket in newBucket)
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
                    Resource = B2ApiResouce.CreateBucket,
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
                var responce = client.Execute<DtoBucket>(req);
                Guard.Against.RestSharpError(responce);
                Guard.Against.B2Error(responce);
                buckets.Add(responce.Data);
            }
            return buckets;
        }

        public static List<DtoBucket> UpdateBucket (List<DtoBucket> bucketsToUpdate)
        {
            var buckets = new List<DtoBucket>();
            foreach (var bucket in bucketsToUpdate)
            {
                var client = new RestClient()
                {
                    BaseUrl = new Uri(Session.Instance.accountSession.apiUrl)
                };
                var req = new RestRequest()
                {
                    Method = Method.POST,
                    Resource = B2ApiResouce.UpdateBucket,
                    RequestFormat = DataFormat.Json
                };
                var body = new Dictionary<string, string>
                {
                    { "accountId", Session.Instance.accountSession.accountId }
                };
                if (!string.IsNullOrEmpty(bucket.bucketName)) { body.Add("bucketName", bucket.bucketName); }
                if (!string.IsNullOrEmpty(bucket.bucketType)) { body.Add("bucketType", bucket.bucketType); }
                if (!string.IsNullOrEmpty(bucket.bucketId)) { body.Add("bucketId", bucket.bucketId); }
                req.AddJsonBody(body);
                req.AddHeader(Constant.Authorization, Session.Instance.accountSession.authorizationToken);
                var responce = client.Execute<DtoBucket>(req);
                Guard.Against.RestSharpError(responce);
                Guard.Against.B2Error(responce);
                buckets.Add(responce.Data);
            }
            return buckets;
        }

        public static List<DtoBucket> RenameBucket (List<DtoBucket> buckets)
        {
            var updatedBuckets = new List<DtoBucket>();
            throw new NotImplementedException();
        }

        public static void SetCorsRules ()
        {
            throw new NotImplementedException();
        }

        public static void SetLifecycleRules ()
        {
            throw new NotImplementedException();
        }

        public static void DeleteBucket (List<DtoBucket> bucketsToDelete)
        {
            var buckets = new List<DtoBucket>();
            foreach (var bucket in bucketsToDelete)
            {
                throw new NotImplementedException();
            }
        }

        internal static string GetBucketIdFromName (string bucketName)
        {
            return ListBuckets(BucketType.All).Find(bucket => bucket.bucketName == bucketName).bucketId;
        }
    }
    //The session data, can be updated but not duplicated
    internal sealed class Session
    {
        public DtoAccount accountSession { get; set; }
        private static readonly Lazy<Session> session = new Lazy<Session>(() => new Session());
        public static Session Instance { get { return session.Value; } }
        private Session() { }
    }
}
