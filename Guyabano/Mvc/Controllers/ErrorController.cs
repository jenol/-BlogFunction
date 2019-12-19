using System;
using System.Net.Http;
using Guyabano.Mvc.HttpBindings;

namespace Guyabano.Mvc.Controllers
{
    internal class ErrorController : Controller
    {
        [Http("GET")]
        [Action("index")]
        public HttpResponseMessage Index()
        {
            var ex = (Exception) ContextItems["LastException"];

            var view = View("error_dev", new
            {
                ex.Message,
                ExceptionTypeName = ex.GetType().Name,
                ex.StackTrace,
                NetVersion = Environment.Version.ToString(),
                Path = "/"
            });

            return view;
        }
    }
}