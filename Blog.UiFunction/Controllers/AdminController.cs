using System.Net.Http;
using System.Threading.Tasks;
using Blog.Service;
using Guyabano.Mvc.HttpBindings;

namespace Blog.UiFunction.Controllers
{
    public class AdminController : UserAwareControllerBase
    {
        public AdminController(IUserService userService) : base(userService) { }

        [Http("GET")]
        [Action("index")]
        public async Task<HttpResponseMessage> Index()
        {
            var user = await GetCurrentUserAsync();
            return user == null
                ? Redirect("login")
                : View("admin", new
                {
                    title = "Test",
                    name = user.FirstName
                });
            ;
        }
    }
}