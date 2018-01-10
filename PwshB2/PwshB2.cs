using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Language;
using PwshB2.Api;
using PwshB2.Api.Dto;

namespace PwshB2
{
    [CmdletBinding(PositionalBinding = true)]
    [Cmdlet(VerbsCommunications.Connect, "B2Service")]
    [OutputType(typeof(DtoAccount))]
    public class ConnectB2Account : PSCmdlet
    {
        [Parameter(HelpMessage = "The AccountId (username) and ApplicationKey (password) for the B2 account.", Mandatory = true)]
        [ValidateNotNullOrEmpty()]
        public PSCredential Credential { get; set; }

        protected override void ProcessRecord()
        {
            var acct = B2.AuthorizeAccount(Credential.UserName, Credential.GetNetworkCredential().Password);
            // Save the session data
            B2.SaveSessionData(acct);
            WriteObject(acct);
        }
    }

    [CmdletBinding(PositionalBinding = true)]
    [Cmdlet(VerbsCommon.Get, "B2Bucket")]
    [OutputType(typeof(DtoBucket[]))]
    public class GetB2Bucket : PSCmdlet
    {
        [Parameter(HelpMessage = "The type of bucket to return.", Mandatory = false)]
        [ArgumentCompleter(typeof(BucketTypeCompleter))]
        public BucketType Type { get; set; } = BucketType.All;

        protected override void BeginProcessing()
        {
            // Might put filtering options here if they exists on B2 API
            base.BeginProcessing();
        }
        protected override void ProcessRecord()
        {
            try
            {
                WriteObject(B2.ListBuckets(Type).ToArray());
            }
            catch (Exception err)
            {
                WriteError(new ErrorRecord(err, "PwshB2GetB2BucketException", ErrorCategory.ConnectionError, null));
            }
        }

        private class BucketTypeCompleter : IArgumentCompleter
        {
            IEnumerable<CompletionResult> IArgumentCompleter.CompleteArgument(string commandName,
                                                                              string parameterName,
                                                                              string wordToComplete,
                                                                              CommandAst commandAst,
                                                                              IDictionary fakeBoundParameters)
            {
                return BucketType.List.Select(item => item.ToString()).ToArray()
                    .Where(new WildcardPattern($"{wordToComplete}*", WildcardOptions.IgnoreCase).IsMatch)
                    .Select(match => new CompletionResult(match));
            }
        }
    }

    [CmdletBinding(PositionalBinding = true)]
    [Cmdlet(VerbsCommon.New, "B2Bucket")]
    [OutputType(typeof(DtoBucket[]))]
    public class NewB2Bucket : PSCmdlet
    {
        [Parameter(HelpMessage = "The name of the new bucket.", Mandatory = true,
            ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateLength(6, 50)]
        public string[] Name { get; set; }

        // The creation of a bucket is limited to public and private
        [ValidateSet("Public", "Private")]
        [Parameter(HelpMessage = "Bucket type.", Mandatory = false)]
        public BucketType Type { get; set; } = BucketType.Private;

        protected override void ProcessRecord()
        {
            try
            {
                var buckets = new List<DtoBucket>();
                var _type = HashMap.FromBucketType[Type];
                foreach (var _name in Name)
                {
                    buckets.Add(new DtoBucket() { bucketName = _name, bucketType = _type });
                }
                WriteObject(B2.CreateBucket(buckets).ToArray());
            }
            catch (Exception err)
            {
                WriteError(new ErrorRecord(err, "PwshB2NewB2BucketException", ErrorCategory.InvalidResult, null));
            }
        }
    }

    [CmdletBinding(PositionalBinding = true)]
    [Cmdlet(VerbsCommon.Set, "B2BucketType")]
    [OutputType(typeof(DtoBucket[]))]
    public class SetB2BucketType : PSCmdlet
    {
        [Parameter(HelpMessage = "The name of the bucket to change.", Mandatory = true,
            ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateLength(6, 50)]
        public string[] Name { get; set; }

        [Parameter(HelpMessage = "Bucket type to set.", Mandatory = false)]
        [ValidateSet("Public", "Private")]
        public BucketType Type { get; set; } = BucketType.Private;

        protected override void ProcessRecord()
        {
            var buckets = new List<DtoBucket>();
            var _type = HashMap.FromBucketType[Type];
            foreach (var _name in Name)
            {
                buckets.Add(new DtoBucket() { bucketType = _type, bucketId = B2.GetBucketIdFromName(_name) });
            }
            WriteObject(B2.UpdateBucket(buckets).ToArray());
        }
    }

    [CmdletBinding(PositionalBinding = true)]
    [Cmdlet(VerbsCommon.Rename, "B2Bucket")]
    [OutputType(typeof(DtoBucket))]
    public class RenameB2Bucket : PSCmdlet
    {
        [Parameter(HelpMessage = "Name of the bucket to rename.", Mandatory = true)]
        [ValidateLength(6, 50)]
        public string Name { get; set; }

        [Parameter(HelpMessage = "The new name of the bucket.", Mandatory = true)]
        [ValidateLength(6, 50)]
        public string NewName { get; set; }

        protected override void ProcessRecord()
        {
            WriteObject(B2.RenameBucket(Name, NewName));
        }
    }
}
