using System;
using System.Threading.Tasks;

namespace Guyabano.Mvc.HttpBindings
{
    public abstract class HttpBindingAttribute : Attribute
    {
        public abstract Task<object> BindModelAsync(HttpBindingContext httpBindingContext);
    }
}