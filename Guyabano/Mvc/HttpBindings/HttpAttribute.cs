using System;

namespace Guyabano.Mvc.HttpBindings
{
    public class HttpAttribute : Attribute
    {
        public HttpAttribute(string httpVerb) => HttpVerb = httpVerb;

        public string HttpVerb { get; }
    }
}