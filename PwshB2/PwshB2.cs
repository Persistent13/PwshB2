using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Language;
using PwshB2.Api;
using PwshB2.Api.Dto;
using PwshB2.Exceptions;

namespace PwshB2
{
    class BucketTypeCompleter : IArgumentCompleter
    {
        IEnumerable<CompletionResult> IArgumentCompleter.CompleteArgument(string commandName,
                                                                          string parameterName,
                                                                          string wordToComplete,
                                                                          CommandAst commandAst,
                                                                          IDictionary fakeBoundParameters)
        {
            var filter = new string[] { BucketType.Public, BucketType.Private };
            return BucketType.List.Select(item => item.ToString())
                .Intersect(filter)
                .Where(new WildcardPattern($"{wordToComplete}*", WildcardOptions.IgnoreCase).IsMatch)
                .Select(match => new CompletionResult(match));
        }
    }

    public class Test
    {
        private static B2 _b2client;

        [CmdletBinding(PositionalBinding = true)]
        [Cmdlet(VerbsCommunications.Connect, "B2Service", ConfirmImpact = ConfirmImpact.None)]
        [OutputType(typeof(Account))]
        public class ConnectB2Service : PSCmdlet
        {
            [Parameter(HelpMessage = "The AccountId (username) and ApplicationKey (password) for the B2 account.", Mandatory = true)]
            [ValidateNotNullOrEmpty()]
            public PSCredential Credential { get; set; }

            protected override void ProcessRecord()
            {
                try
                {
                    _b2client = new B2(Credential.UserName, Credential.GetNetworkCredential().Password);
                    WriteObject(Session.Instance.accountSession);
                }
                catch
                {
                    throw new Exception();
                }
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
                    WriteObject(_b2client.ListBuckets(Type).ToArray());
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

            [Parameter(HelpMessage = "Bucket type.", Mandatory = false)]
            [ArgumentCompleter(typeof(BucketTypeCompleter))]
            public BucketType Type { get; set; } = BucketType.Private;

            protected override void ProcessRecord()
            {
                foreach (var _name in Name)
                {
                    try
                    {
                        WriteObject(_b2client.CreateBucket(_name, Type));
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
        [Cmdlet(VerbsCommon.Set, "B2BucketType", ConfirmImpact = ConfirmImpact.High)]
        public class SetB2BucketType : PSCmdlet
        {
            [Parameter(HelpMessage = "The name of the bucket to change.", Mandatory = true,
                ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
            [ValidateLength(6, 50)]
            public string[] Name { get; set; }

            [Parameter(HelpMessage = "Bucket type to set.", Mandatory = false)]
            [ArgumentCompleter(typeof(BucketTypeCompleter))]
            public BucketType Type { get; set; } = BucketType.Private;

            protected override void ProcessRecord()
            {
                foreach (var _name in Name)
                {
                    try
                    {
                        if (ShouldProcess(_name, $"Change bucket type to: {Type}"))
                        {
                            _b2client.SetBucketType(_name, Type);
                        }
                    }
                    catch (B2ObjectNotFound err)
                    {
                        WriteError(new ErrorRecord(err, "PwshB2ObjectNotFound", ErrorCategory.ObjectNotFound, _name));
                    }
                }
            }
        }

        [CmdletBinding(PositionalBinding = true)]
        [Cmdlet(VerbsCommon.Remove, "B2Bucket", ConfirmImpact = ConfirmImpact.High)]
        public class RemoveB2Bucket : PSCmdlet
        {
            [Parameter(HelpMessage = "The name of the bucket to remove.", Mandatory = true,
                ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
            [ValidateLength(6, 50)]
            public string[] Name { get; set; }


            protected override void ProcessRecord()
            {
                foreach (var _name in Name)
                {
                    try
                    {
                        if (ShouldProcess(_name, "Remove bucket."))
                        {
                            _b2client.RemoveBucket(_name);
                        }
                    }
                    catch (B2HttpException err)
                    {
                        WriteError(new ErrorRecord(err, "PwshB2RemoveB2BucketException", ErrorCategory.ConnectionError, _name));
                    }
                    catch (B2Exception err)
                    {
                        WriteError(new ErrorRecord(err, "PwshB2RemoveB2BucketException", ErrorCategory.InvalidOperation, _name));
                    }
                    catch (Exception err)
                    {
                        WriteError(new ErrorRecord(err, "PwshB2RemoveB2BucketException", ErrorCategory.InvalidResult, _name));
                    }
                }
            }
        }
    }
}
