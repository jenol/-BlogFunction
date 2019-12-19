using System.Threading.Tasks;
using System.Web;

namespace Guyabano.Mvc.HttpBindings
{
    public class FromQueryAttribute : HttpBindingAttribute
    {
        public FromQueryAttribute(string parameterName) => ParameterName = parameterName;

        public string ParameterName { get; }

        public override Task<object> BindModelAsync(HttpBindingContext httpBindingContext)
        {
            var query = HttpUtility.ParseQueryString(httpBindingContext.Request.RequestUri.Query);
            return Task.FromResult((object) query[ParameterName]);
        }
    }
}