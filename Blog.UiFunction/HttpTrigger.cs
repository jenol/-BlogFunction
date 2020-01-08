using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Blog.Service;
using Guyabano.Mvc.Controllers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Blog.UiFunction
{
    public class HttpTrigger
    {
        private readonly IControllerFactory _controllerFactory;
        private readonly IUserService _userService;

        public HttpTrigger(IUserService userService, IControllerFactory controllerFactory)
        {
            _userService = userService;
            _controllerFactory = controllerFactory;
        }

        [FunctionName("BlogUi")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
            HttpRequestMessage req, ILogger log)
        {
            try
            {
                var query = HttpUtility.ParseQueryString(req.RequestUri.Query);

                switch (query["path"])
                {
                    case "login/logout":
                    {
                        var controller = _controllerFactory.CreateController("login", req, null);
                        return await controller.InvokeActionAsync("login/logout", req.Method);
                    }
                    case "login":
                    {
                        var controller = _controllerFactory.CreateController("login", req, null);
                        if (req.Method == HttpMethod.Get)
                        {
                            return await controller.InvokeActionAsync("index", req.Method);
                        }

                        return await controller.InvokeActionAsync("login", req.Method);
                    }
                    case "admin":
                    {
                        var controller = _controllerFactory.CreateController("admin", req, null);
                        if (req.Method == HttpMethod.Get)
                        {
                            return await controller.InvokeActionAsync("index", req.Method);
                        }

                        return await controller.InvokeActionAsync("login", req.Method);
                    }
                    default:
                    {
                        var controller = _controllerFactory.CreateController("home", req, null);
                        //log.Send.Info("C# HTTP trigger function processed a request.");

                        //string requestBody = new StreamReader(req.Body).ReadToEnd();
                        //dynamic data = JsonConvert.DeserializeObject(requestBody);
                        //name = name ?? data?.name;

                        if (req.Method == HttpMethod.Get)
                        {
                            return await controller.InvokeActionAsync("index", req.Method);
                        }

                        return await controller.InvokeActionAsync("submit", req.Method);
                    }
                }
            }
            catch (Exception ex)
            {
                //log.Error(ex.Message, ex);
                return await _controllerFactory.CreateController("_error", req, new Dictionary<string, object>
                {
                    {"LastException", ex}
                }).InvokeActionAsync("index", req.Method);
            }
        }
    }
}