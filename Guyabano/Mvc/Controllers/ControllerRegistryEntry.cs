using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Guyabano.Mvc.Controllers
{
    internal class ControllerRegistryEntry
    {
        public Func<Controller> Create { get; set; }

        public Dictionary<string, Func<object, HttpRequestMessage, Task<HttpResponseMessage>>> ActionMappings
        {
            get;
            set;
        }
    }
}