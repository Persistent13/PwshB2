using System;

namespace PwshB2.Exceptions
{
    public class B2Exception : Exception
    {
        public B2Exception () { }
        public B2Exception (string message) : base (message) { }
        public B2Exception (string message, Exception inner) : base (message, inner) { }
    }
    public class B2NameException : Exception
    {
        public B2NameException () { }
        public B2NameException (string message) : base (message) { }
        public B2NameException (string message, Exception inner) : base (message, inner) { }
    }
    public class B2HttpException : Exception
    {
        public B2HttpException () { }
        public B2HttpException (string message) : base(message) { }
        public B2HttpException (string message, Exception inner) : base(message, inner) { }
    }
    public class B2InvalidCountException : Exception
    {
        public B2InvalidCountException () { }
        public B2InvalidCountException (string message) : base(message) { }
        public B2InvalidCountException (string message, Exception inner) : base(message, inner) { }
    }
    public class B2InvalidHeaderException : Exception
    {
        public B2InvalidHeaderException () { }
        public B2InvalidHeaderException (string message) : base(message) { }
        public B2InvalidHeaderException (string message, Exception inner) : base(message, inner) { }
    }
    public class B2InvalidLifecycleRuleException : Exception
    {
        public B2InvalidLifecycleRuleException () { }
        public B2InvalidLifecycleRuleException (string message) : base(message) { }
        public B2InvalidLifecycleRuleException (string message, Exception inner) : base(message, inner) { }
    }
    public class B2ObjectNotFound : Exception
    {
        public B2ObjectNotFound () { }
        public B2ObjectNotFound (string message) : base(message) { }
        public B2ObjectNotFound (string message, Exception inner) : base(message, inner) { }
    }
    public class B2BucketTypeError : Exception
    {
        public B2BucketTypeError() { }
        public B2BucketTypeError(string message) : base(message) { }
        public B2BucketTypeError(string message, Exception inner) : base(message, inner) { }
    }
}
