using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Guyabano.Mvc.HttpBindings
{
    public class FromFormAttribute : HttpBindingAttribute
    {
        public override async Task<object> BindModelAsync(HttpBindingContext httpBindingContext)
        {
            var model = Activator.CreateInstance(httpBindingContext.TargetType);
            var content = await httpBindingContext.Request.Content.ReadAsStringAsync();
            var pairs = content.Split('&');

            foreach (var pair in pairs)
            {
                var kv = pair.Split('=');

                var property =
                    httpBindingContext.TargetType.GetProperty(kv[0], BindingFlags.Instance | BindingFlags.Public);

                if (property == null)
                {
                    continue;
                }

                property.SetValue(model, kv[1]);
            }

            return model;
        }
    }
}