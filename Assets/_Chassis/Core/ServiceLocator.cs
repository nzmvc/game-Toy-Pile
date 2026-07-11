using System;
using System.Collections.Generic;

namespace Chassis.Core
{
    /// <summary>
    /// A simple and lightweight service locator for runtime dependency resolution.
    /// Used for retrieving systems like AdsManager, SaveSystem, Analytics, etc.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public static void Register<T>(T service)
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                _services[type] = service;
            }
            else
            {
                _services.Add(type, service);
            }
        }

        public static T Get<T>()
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var service))
            {
                return (T)service;
            }
            throw new Exception($"Service of type {type.Name} is not registered.");
        }

        public static bool TryGet<T>(out T service)
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var s))
            {
                service = (T)s;
                return true;
            }
            service = default;
            return false;
        }

        public static void Unregister<T>()
        {
            var type = typeof(T);
            _services.Remove(type);
        }

        public static void Clear()
        {
            _services.Clear();
        }
    }
}
