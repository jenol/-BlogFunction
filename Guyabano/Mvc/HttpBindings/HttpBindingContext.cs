using System;
using System.Net.Http;

namespace Guyabano.Mvc.HttpBindings
{
    public class HttpBindingContext
    {
        public HttpBindingContext(HttpRequestMessage requestMessage, Type targetType)
        {
            Request = requestMessage;
            TargetType = targetType;
        }

        public HttpRequestMessage Request { get; }

        public Type TargetType { get; }
    }
}