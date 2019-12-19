using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blog.UiFunction.Models;
using Guyabano.Mvc.Controllers;
using Guyabano.Mvc.HttpBindings;

namespace Blog.UiFunction.Controllers
{
    public class HomeController : Controller
    {
        [Http("GET")]
        [Action("index")]
        public async Task<HttpResponseMessage> IndexAsync([FromQuery("name")] string name)
        {
            var view = View("index", new
            {
                title = "Test",
                name,
                question = "Do you want to attend the Staff Christmas Staff Party?",
                description = ""
            });

            await Task.Delay(TimeSpan.FromSeconds(1));

            return view;
        }

        [Http("POST")]
        [Action("submit")]
        public HttpResponseMessage Submit([FromForm] Login login) =>
            View("index", new
            {
                title = "Test",
                name = login.UserName,
                question = "ssssssssssssssssssssss",
                description = ""
            });

        [Http("GET")]
        [Action("thankyou")]
        public HttpResponseMessage ThankYou() =>
            View("thankyou", new
            {
                title = "Test"
            });
    }
}