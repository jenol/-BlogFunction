using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blog.Service;
using Blog.UiFunction.Models;
using Guyabano.Mvc.HttpBindings;

namespace Blog.UiFunction.Controllers
{
    public class LoginController : UserAwareControllerBase
    {
        public LoginController(IAuthenticationService authenticationService) : base(authenticationService) { }

        [Http("GET")]
        [Action("index")]
        public async Task<HttpResponseMessage> IndexAsync()
        {
            var user = await GetCurrentUserAsync();

            if (user != null)
            {
                return Redirect("admin");
            }

            var view = View("login", new
            {
                title = "Test",
                name = ""
            });

            await Task.Delay(TimeSpan.FromSeconds(1));

            return view;
        }

        [Http("POST")]
        [Action("login")]
        public async Task<HttpResponseMessage> LoginAsync([FromForm] Login login)
        {
            var user = await GetCurrentUserAsync();

            if (user != null)
            {
                return Redirect("admin");
            }

            var token = await AuthenticationService.GetSecurityTokenAsync(login.UserName, login.Password);

            if (token == null)
            {
                return Redirect("login");
            }

            var response = Redirect("admin");

            var cookie = new CookieHeaderValue("session-id", token)
            {
                Expires = DateTimeOffset.Now.AddDays(1),
                Domain = Request.RequestUri.Host,
                Path = "/",
                HttpOnly = true
            };

            response.Headers.AddCookies(new[] {cookie});

            return response;
        }

        [Http("GET")]
        [Action("login/logout")]
        public HttpResponseMessage Logout()
        {
            var response = Redirect("login");

            var cookie = new CookieHeaderValue("session-id", "")
            {
                Expires = DateTimeOffset.Now.AddDays(-1),
                Domain = Request.RequestUri.Host,
                Path = "/",
                HttpOnly = true
            };

            response.Headers.AddCookies(new[] {cookie});

            return response;
        }
    }
}