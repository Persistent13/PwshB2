using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Language;
using PwshB2.Api;

namespace PwshB2
{
    [CmdletBinding(PositionalBinding = true)]
    [Cmdlet(VerbsCommunications.Connect, "B2Service")]
    [OutputType(typeof(Account))]
    public class ConnectB2Account : PSCmdlet
    {
        [Parameter(HelpMessage = "The AccountId (username) and ApplicationKey (password) for the B2 account.", Mandatory = true)]
        [ValidateNotNullOrEmpty()]
        public PSCredential Credential { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }
        protected override void ProcessRecord()
        {
            var acct = B2.AuthorizeAccount(Credential.UserName, Credential.GetNetworkCredential().Password);
            // Save the session data
            B2.SaveSessionData(acct);
            WriteObject(acct);
        }
        protected override void EndProcessing()
        {
            base.EndProcessing();
        }
    }

    [CmdletBinding(PositionalBinding = true)]
    [Cmdlet(VerbsCommon.Get, "B2Bucket")]
    [OutputType(typeof(Bucket[]))]
    public class GetB2Bucket : PSCmdlet
    {
        [Parameter(HelpMessage = "The type of bucket to return.", Mandatory = false)]
        [ArgumentCompleter(typeof(BucketTypeCompleter))]
        public BucketEnum Type { get; set; } = BucketEnum.all;

        protected override void BeginProcessing()
        {
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
                return Enum.GetNames(typeof(BucketEnum))
                    .Where(new WildcardPattern(wordToComplete + "*", WildcardOptions.IgnoreCase).IsMatch)
                    .Select(x => new CompletionResult(x));
            }
        }
    }
}
