using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blog.Service;
using Guyabano.Mvc.Controllers;

namespace Blog.UiFunction.Controllers
{
    public abstract class UserAwareControllerBase : Controller
    {
        protected UserAwareControllerBase(IUserService userService) => UserService = userService;

        protected IUserService UserService { get; }

        protected async Task<User> GetCurrentUserAsync()
        {
            var cookie = Request.Headers.GetCookies().SelectMany(c => c.Cookies)
                .FirstOrDefault(c => c.Name == "session-id");

            if (cookie?.Value == null)
            {
                return null;
            }

            try
            {
                return await UserService.GetUserBySecurityTokenAsync(cookie.Value);
            }
            catch (Exception ex)
            {
                var p = 0;
            }

            return null;
        }
    }
}