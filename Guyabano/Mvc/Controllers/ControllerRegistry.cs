using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Guyabano.Mvc.Controllers
{
    internal class ControllerRegistry
    {
        private readonly Dictionary<string, ControllerRegistryEntry> _registry =
            new Dictionary<string, ControllerRegistryEntry>();

        public ControllerRegistryEntry this[string controllerName] => _registry[controllerName];

        public void Add(string controllerName, Func<Controller> factory,
            Dictionary<string, Func<object, HttpRequestMessage, Task<HttpResponseMessage>>> actionMappings)
        {
            _registry.Add(controllerName, new ControllerRegistryEntry
            {
                Create = factory,
                ActionMappings = actionMappings
            });
        }

        public bool Contains(string controllerName) => _registry.ContainsKey(controllerName);
    }
}