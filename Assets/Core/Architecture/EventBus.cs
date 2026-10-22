using System;
using System.Collections.Generic;

namespace Core.Architecture
{
    /// <summary>
    /// Contract interface for struct-based zero-allocation event payloads.
    /// </summary>
    public interface IGameEvent
    {
    }

    /// <summary>
    /// High-throughput, type-safe Event Bus supporting zero GC allocation struct events.
    /// Eliminates tight couplings between UI, VR inputs, audio, and gameplay modules.
    /// </summary>
    public static class EventBus
    {
        private interface IEventBinding
        {
        }

        private sealed class EventBinding<T> : IEventBinding where T : struct, IGameEvent
        {
            public Action<T> Callbacks;
        }

        private static readonly Dictionary<Type, IEventBinding> Bindings = new(128);
        private static readonly object SyncLock = new();

        /// <summary>
        /// Subscribes a listener action to an event type.
        /// </summary>
        public static void Subscribe<T>(Action<T> callback) where T : struct, IGameEvent
        {
            if (callback == null) return;
            lock (SyncLock)
            {
                var type = typeof(T);
                if (!Bindings.TryGetValue(type, out var binding))
                {
                    binding = new EventBinding<T>();
                    Bindings[type] = binding;
                }

                ((EventBinding<T>)binding).Callbacks += callback;
            }
        }

        /// <summary>
        /// Unsubscribes a listener from an event type.
        /// </summary>
        public static void Unsubscribe<T>(Action<T> callback) where T : struct, IGameEvent
        {
            if (callback == null) return;
            lock (SyncLock)
            {
                var type = typeof(T);
                if (Bindings.TryGetValue(type, out var binding))
                {
                    ((EventBinding<T>)binding).Callbacks -= callback;
                }
            }
        }

        /// <summary>
        /// Publishes an event payload to all registered subscribers with zero boxing allocations.
        /// </summary>
        public static void Publish<T>(in T eventData) where T : struct, IGameEvent
        {
            Action<T> localCallbacks = null;
            lock (SyncLock)
            {
                if (Bindings.TryGetValue(typeof(T), out var binding))
                {
                    localCallbacks = ((EventBinding<T>)binding).Callbacks;
                }
            }

            localCallbacks?.Invoke(eventData);
        }

        /// <summary>
        /// Clears all event subscriptions across the entire application.
        /// </summary>
        public static void Clear()
        {
            lock (SyncLock)
            {
                Bindings.Clear();
            }
        }
    }
}
