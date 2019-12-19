using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Guyabano.Mvc.Controllers;
using Guyabano.Mvc.Views;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Guyabano.DepenencyInjection.Mvc
{
    public static class MvcBuilderExtensions
    {
        public static void RegisterControllers(this IFunctionsHostBuilder builder, Assembly assembly)
        {
            var tp = assembly.GetTypes().ToArray();
            var controllers = tp
                .Where(t => typeof(Controller).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToDictionary(c => c.Name.Replace("Controller", "").ToLower(), c => c);

            builder.Services.AddTransient(typeof(ErrorController));

            builder.Services.AddSingleton<IControllerFactory>(p =>
            {
                var controllerFactory = new ControllerFactory();

                controllerFactory.RegisterController(typeof(ErrorController), "_error",
                    () => (Controller) p.GetService(typeof(ErrorController)));

                foreach (var name in controllers.Keys)
                {
                    var controllerType = controllers[name];
                    controllerFactory.RegisterController(controllerType, name,
                        () => (Controller) p.GetService(controllerType));
                }

                return controllerFactory;
            });

            foreach (var controllerType in controllers.Values)
            {
                builder.Services.AddTransient(controllerType);
            }
        }

        public static void RegisterViews(this IFunctionsHostBuilder builder, Assembly assembly,
            IEnumerable<(string Name, string ResourceName)> views)
        {
            ViewFactory.Instance.RegisterView("error_dev", "Mvc/Views/_error_dev.liquid",
                typeof(MvcBuilderExtensions).Assembly);

            foreach (var view in views)
            {
                ViewFactory.Instance.RegisterView(view.Name, view.ResourceName, assembly);
            }
        }
    }
}