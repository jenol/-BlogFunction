using System.Collections.Generic;
using System.Net.Http;

namespace Guyabano.Mvc.Controllers
{
    public interface IControllerFactory
    {
        Controller CreateController(string name, HttpRequestMessage requestMessage,
            IDictionary<string, object> contextItems);
    }
}