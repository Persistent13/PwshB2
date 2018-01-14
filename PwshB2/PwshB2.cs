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
using PwshB2.Exceptions;

namespace PwshB2
{
    [CmdletBinding(PositionalBinding = true)]
    [Cmdlet(VerbsCommunications.Connect, "B2Service")]
    [OutputType(typeof(Account))]
    public class ConnectB2Service : PSCmdlet
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
    [OutputType(typeof(Bucket[]))]
    public class GetB2Bucket : PSCmdlet
    {
        [Parameter(HelpMessage = "The type of bucket to return.", Mandatory = false)]
        [ArgumentCompleter(typeof(BucketTypeCompleter))]
        public BucketType Type { get; set; } = BucketType.All;

        protected override void BeginProcessing()
        {
            // Might put filtering options here if they exists on B2 API
            // e.g. Name, size, etc.
        }
        protected override void ProcessRecord()
        {
            try
            {
                WriteObject(B2.ListBuckets(Type).ToArray());
            }
            catch (B2HttpException err)
            {
                WriteError(new ErrorRecord(err, "PwshB2GetB2BucketException", ErrorCategory.ConnectionError, null));
            }
            catch (B2Exception err)
            {
                WriteError(new ErrorRecord(err, "PwshB2GetB2BucketException", ErrorCategory.ConnectionError, null));
            }
            catch (Exception err)
            {
                WriteError(new ErrorRecord(err, "PwshB2GetB2BucketException", ErrorCategory.InvalidResult, null));
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
    [OutputType(typeof(Bucket[]))]
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
            foreach (var _name in Name)
            {
                try
                {
                    WriteObject(B2.CreateBucket(_name, Type));
                }
                catch (B2HttpException err)
                {
                    WriteError(new ErrorRecord(err, "PwshB2GetB2BucketException", ErrorCategory.ConnectionError, _name));
                }
                catch (B2Exception err)
                {
                    WriteError(new ErrorRecord(err, "PwshB2GetB2BucketException", ErrorCategory.ConnectionError, _name));
                }
                catch (Exception err)
                {
                    WriteError(new ErrorRecord(err, "PwshB2NewB2BucketException", ErrorCategory.InvalidResult, _name));
                }
            }
        }
    }

    [CmdletBinding(PositionalBinding = true)]
    [Cmdlet(VerbsCommon.Set, "B2BucketType")]
    [OutputType(typeof(Bucket[]))]
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
            foreach (var _name in Name)
            {
                try
                {
                    B2.SetBucketType(_name, Type);
                }
                catch (B2ObjectNotFound err)
                {
                    WriteError(new ErrorRecord(err, "PwshB2ObjectNotFound", ErrorCategory.ObjectNotFound, _name));
                }
            }
        }
    }

    [CmdletBinding(PositionalBinding = true)]
    [Cmdlet(VerbsCommon.Rename, "B2Bucket")]
    [OutputType(typeof(Bucket))]
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
            throw new PSNotImplementedException();
            //WriteObject(B2.RenameBucket(Name, NewName));
        }
    }
}
