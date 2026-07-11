using System;
using System.Collections.Generic;

namespace Chassis.Core
{
    /// <summary>
    /// Type-safe, GC-friendly EventBus using struct events.
    /// Exposes a clean API for publisher-subscriber pattern.
    /// </summary>
    public static class EventBus
    {
        public static void Register<T>(Action<T> listener) where T : struct
        {
            EventBusInternal<T>.Register(listener);
        }

        public static void Unregister<T>(Action<T> listener) where T : struct
        {
            EventBusInternal<T>.Unregister(listener);
        }

        public static void Publish<T>(T ev) where T : struct
        {
            EventBusInternal<T>.Publish(ev);
        }
    }

    /// <summary>
    /// Internal generic implementation to avoid dictionary lookups and type casting.
    /// </summary>
    /// <typeparam name="T">Struct event type</typeparam>
    internal static class EventBusInternal<T> where T : struct
    {
        private static Action<T> _listeners;

        public static void Register(Action<T> listener)
        {
            _listeners += listener;
        }

        public static void Unregister(Action<T> listener)
        {
            _listeners -= listener;
        }

        public static void Publish(T ev)
        {
            _listeners?.Invoke(ev);
        }
    }
}
