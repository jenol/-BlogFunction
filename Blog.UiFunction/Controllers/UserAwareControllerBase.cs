using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blog.Service;
using Blog.Service.DomainObjects;
using Guyabano.Mvc.Controllers;

namespace Blog.UiFunction.Controllers
{
    public abstract class UserAwareControllerBase : Controller
    {
        protected UserAwareControllerBase(IAuthenticationService authenticationService) => AuthenticationService = authenticationService;

        protected IAuthenticationService AuthenticationService { get; }

        protected async Task<LoginDetails> GetCurrentUserAsync()
        {
            var cookie = Request.Headers.GetCookies().SelectMany(c => c.Cookies)
                .FirstOrDefault(c => c.Name == "session-id");

            if (cookie?.Value == null)
            {
                return null;
            }

            try
            {
                return await AuthenticationService.GetLoginAsync(cookie.Value);
            }
            catch (Exception ex)
            {
                var p = 0;
            }

            return null;
        }
    }
}