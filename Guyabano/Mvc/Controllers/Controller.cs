using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Guyabano.Mvc.Views;

namespace Guyabano.Mvc.Controllers
{
    public abstract class Controller
    {
        public HttpRequestMessage Request { get; internal set; }
        public IDictionary<string, object> ContextItems { get; internal set; }

        internal Dictionary<string, Func<object, HttpRequestMessage, Task<HttpResponseMessage>>> ActionMappings
        {
            get;
            set;
        }

        public Task<HttpResponseMessage> InvokeActionAsync(HttpMethod httpMethod) =>
            InvokeActionAsync("index", httpMethod);

        public Task<HttpResponseMessage> InvokeActionAsync(string actionName) =>
            InvokeActionAsync(actionName, HttpMethod.Get);

        public async Task<HttpResponseMessage> InvokeActionAsync(string actionName, HttpMethod httpMethod)
            => ActionMappings.TryGetValue(actionName, out var func)
                ? await func(this, Request)
                : new HttpResponseMessage(HttpStatusCode.NotFound);

        protected HttpResponseMessage Redirect(string path)
        {
            var urlBuilder = new UriBuilder(Request.RequestUri.Scheme,
                Request.RequestUri.Host,
                Request.RequestUri.Port,
                Request.RequestUri.AbsolutePath)
            {
                Query = $"?path={path}"
            };

            var response = new HttpResponseMessage(HttpStatusCode.Redirect);
            response.Headers.Location = urlBuilder.Uri;
            return response;
        }

        protected HttpResponseMessage View(string name, dynamic model)
        {
            var html = ViewFactory.Instance.GetView(name, model);
            var response = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(html)};
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        internal static Task<HttpResponseMessage> InvokeActionAsync(object obj, MethodInfo methodInfo,
            List<Func<HttpRequestMessage, Task<object>>> methodParams, HttpRequestMessage request)
        {
            async Task<object[]> InitParametersAsync()
            {
                var methodTasks = methodParams.Select(methodParameterInitializer => methodParameterInitializer(request))
                    .ToArray();
                await Task.WhenAll(methodTasks);
                return methodTasks.Select(t => t.Result).ToArray();
            }

            async Task<HttpResponseMessage> InvokeAsync()
            {
                var methodParameters = await InitParametersAsync();
                return await (Task<HttpResponseMessage>) methodInfo.Invoke(obj, methodParameters);
            }

            async Task<HttpResponseMessage> InvokeSync()
            {
                var methodParameters = await InitParametersAsync();
                return methodInfo.Invoke(obj, methodParameters) as HttpResponseMessage;
            }

            return methodInfo.ReturnType == typeof(Task<HttpResponseMessage>) ? InvokeAsync() : InvokeSync();
        }
    }
}