using System.Reflection;

namespace Guyabano.Mvc.Views
{
    public interface IViewFactory
    {
        void RegisterView(string name, string path, Assembly assembly);

        string GetView<T>(string name, T model);
    }
}