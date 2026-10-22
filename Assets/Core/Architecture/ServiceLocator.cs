using System;
using System.Collections.Generic;

namespace Core.Architecture
{
    /// <summary>
    /// Thread-safe, high-performance Service Locator pattern implementation for runtime dependency injection.
    /// Provides zero-allocation lookup across decoupled subsystems.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, IGameService> Services = new(64);
        private static readonly object SyncRoot = new();

        /// <summary>
        /// Registers a service implementation against contract interface T.
        /// </summary>
        public static void Register<T>(T service) where T : class, IGameService
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            lock (SyncRoot)
            {
                var type = typeof(T);
                if (Services.ContainsKey(type))
                {
                    UnityEngine.Debug.LogWarning($"[ServiceLocator] Overwriting existing service registration for {type.Name}");
                }
                Services[type] = service;
                service.Initialize();
            }
        }

        /// <summary>
        /// Retrieves the registered service instance for contract interface T.
        /// </summary>
        public static T Get<T>() where T : class, IGameService
        {
            lock (SyncRoot)
            {
                var type = typeof(T);
                if (Services.TryGetValue(type, out var service))
                {
                    return (T)service;
                }
                throw new InvalidOperationException($"[ServiceLocator] Service of type {type.Name} is not registered.");
            }
        }

        /// <summary>
        /// Attempts to retrieve the service instance if registered.
        /// </summary>
        public static bool TryGet<T>(out T service) where T : class, IGameService
        {
            lock (SyncRoot)
            {
                if (Services.TryGetValue(typeof(T), out var s))
                {
                    service = (T)s;
                    return true;
                }
                service = null;
                return false;
            }
        }

        /// <summary>
        /// Unregisters and disposes the specified service contract.
        /// </summary>
        public static void Unregister<T>() where T : class, IGameService
        {
            lock (SyncRoot)
            {
                var type = typeof(T);
                if (Services.TryGetValue(type, out var service))
                {
                    service.Dispose();
                    Services.Remove(type);
                }
            }
        }

        /// <summary>
        /// Clears and disposes all registered services. Useful during domain reload or teardown.
        /// </summary>
        public static void Reset()
        {
            lock (SyncRoot)
            {
                foreach (var service in Services.Values)
                {
                    service?.Dispose();
                }
                Services.Clear();
            }
        }
    }
}
