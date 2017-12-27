using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using Ardalis.GuardClauses;
using Newtonsoft.Json;

namespace PwshB2.Api
{
    public class B2
    {
        public static Account AuthorizeAccount (string userName, string password)
        {
            var client = new RestClient(Constant.BaseApiUrl)
            {
                Authenticator = new HttpBasicAuthenticator(userName, password)
            };
            var req = new RestRequest()
            {
                Method = Method.GET,
                Resource = Constant.AuthorizeAccountResource,
                RequestFormat = DataFormat.Json
            };
            var responce = client.Execute<Account>(req);
            Guard.Against.Null(responce.Data, nameof(responce.Data));
            return responce.Data;
        }
        public static void SaveSessionData (Account account)
        {
            Session.Instance.accountSession = account;
        }
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
                RequestFormat = DataFormat.Json,
                OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; }
            };
            var body = new Dictionary<string, string>
            {
                { "accountId", Session.Instance.accountSession.accountId }
            };
            req.AddJsonBody(body);
            req.AddHeader(Constant.Authorization, Session.Instance.accountSession.authorizationToken);
            var responce = client.Execute<Buckets>(req);
            Guard.Against.B2Error(responce);
            Guard.Against.RestSharpError(responce);
            return responce.Data.buckets;
        }
    }
    public sealed class Session
    {
        public Account accountSession { get; set; }
        private static readonly Lazy<Session> session = new Lazy<Session>(() => new Session());
        public static Session Instance { get { return session.Value; } }
        private Session() { }
    }
}
