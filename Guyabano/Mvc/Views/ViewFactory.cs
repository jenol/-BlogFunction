using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DotLiquid;
using DotLiquid.FileSystems;

namespace Guyabano.Mvc.Views
{
    public class ViewFactory : IViewFactory, IFileSystem
    {
        private static readonly Dictionary<string, Lazy<Template>>
            _viewFuncs = new Dictionary<string, Lazy<Template>>();

        private static readonly Lazy<IViewFactory> _instance = new Lazy<IViewFactory>(() => new ViewFactory());

        private ViewFactory() => Template.FileSystem = this;

        public static IViewFactory Instance => _instance.Value;

        string IFileSystem.ReadTemplateFile(Context context, string templateName) =>
            GetView<dynamic>(templateName.Trim('"', '\''), new { });

        public void RegisterView(string name, string path, Assembly assembly)
        {
            if (_viewFuncs.ContainsKey(name))
            {
                return;
            }

            var resourceName = assembly.GetName().Name + "." + path.Replace("/", ".");

            if (!assembly.GetManifestResourceNames().Contains(resourceName))
            {
                throw new Exception($"Resource {resourceName} not found for path {path}");
            }

            _viewFuncs.Add(name, new Lazy<Template>(() => GetFromResource(resourceName, assembly)));
        }

        public string GetView<T>(string name, T model)
        {
            if (!_viewFuncs.TryGetValue(name, out var lazyView))
            {
                return $"View not found {name}";
            }

            var hash = Hash.FromAnonymousObject(model);
            var view = lazyView.Value;
            return view.Render(hash);
        }

        private Template GetFromResource(string resourceName, Assembly assembly)
        {
            var viewText = GetViewResource(resourceName, assembly);
            return Template.Parse(viewText);
        }

        private string GetViewResource(string resourceName, Assembly assembly)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}