using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Guyabano.Mvc.HttpBindings;

namespace Guyabano.Mvc.Controllers
{
    internal class ControllerFactory : IControllerFactory
    {
        private readonly ControllerRegistry _controllerRegistry = new ControllerRegistry();

        public Controller CreateController(string name, HttpRequestMessage requestMessage,
            IDictionary<string, object> contextItems)
        {
            var entry = _controllerRegistry[name];
            var controller = entry.Create();
            controller.Request = requestMessage;
            controller.ContextItems = contextItems ?? new Dictionary<string, object>();
            controller.ActionMappings = entry.ActionMappings;
            return controller;
        }

        internal void RegisterController(Type controllerType, string name, Func<object> func)
        {
            if (_controllerRegistry.Contains(name))
            {
                return;
            }

            var actions = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(
                m => m.DeclaringType == controllerType && (m.ReturnType == typeof(HttpResponseMessage) ||
                                                           m.ReturnType == typeof(Task<HttpResponseMessage>)));

            var p = new Dictionary<string, Func<object, HttpRequestMessage, Task<HttpResponseMessage>>>();

            foreach (var action in actions)
            {
                var httpVerb = action.GetCustomAttribute<HttpAttribute>()?.HttpVerb ?? "GET";
                var actionName = action.GetCustomAttributes().OfType<ActionAttribute>().FirstOrDefault()?.ActionName ??
                                 action.Name.ToLower();

                var l = new List<Func<HttpRequestMessage, Task<object>>>();

                foreach (var param in action.GetParameters())
                {
                    var attr = param.GetCustomAttributes().OfType<HttpBindingAttribute>().FirstOrDefault();

                    if (attr != null)
                    {
                        l.Add(r => attr.BindModelAsync(new HttpBindingContext(r, param.ParameterType)));
                    }
                    else
                    {
                        l.Add(r => Task.FromResult(param.ParameterType.IsValueType
                            ? Activator.CreateInstance(param.ParameterType)
                            : null));
                    }
                }

                p.Add(actionName, (o, r) => Controller.InvokeActionAsync(o, action, l, r));
            }

            if (_controllerRegistry.Contains(name))
            {
                return;
            }

            _controllerRegistry.Add(name, () => (Controller) func(), p);
        }
    }
}