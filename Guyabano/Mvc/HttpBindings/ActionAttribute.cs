using System;

namespace Guyabano.Mvc.HttpBindings
{
    public class ActionAttribute : Attribute
    {
        public ActionAttribute(string actionName) => ActionName = actionName;

        public string ActionName { get; }
    }
}