using System;
using System.Collections.Generic;
using System.Text;

namespace IMMLSharp
{
    /// <summary>
    /// Basic Dependency Injection implementation
    /// </summary>
    public static class DIContainer
    {
        private static Dictionary<Type, Lazy<object>> _Services = new Dictionary<Type, Lazy<object>>();        

        public static void Register<T, U>(U init)
        {
            _Services.Add(typeof(T), new Lazy<object>(() => init));
        }

        public static T Get<T>()
        {
            return (T)_Services[typeof(T)].Value;
        }
    }
}
