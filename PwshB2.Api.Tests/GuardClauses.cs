using System;
using Xunit;
using Ardalis.GuardClauses;
using RestSharp;
using System.Collections.Generic;
using System.Net;

namespace PwshB2.Api.Tests
{
    public class GuardClauses
    {
        [Theory]
        [InlineData()]
        public void ValidateB2ErrorClause()
        {
            Guard.Against.B2Error();
        }
    }
}
