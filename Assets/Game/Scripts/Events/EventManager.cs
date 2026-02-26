using System;
using System.Collections.Generic;

namespace Game.Scripts.Events {
    public static class EventManager {

        private static Dictionary<Type, Delegate> Events = new Dictionary<Type, Delegate>();


        public static void Publish<T>(T eventData) {
            var type = typeof(T);

            if (Events.TryGetValue(type, out var callback)) {
                ((Action<T>)callback)?.Invoke(eventData);
            }
        }
        
        public static void Subscribe<T>(Action<T> callback) {
            var type = typeof(T);

            if (Events.TryGetValue(type, out var existing)) {
                Events[type] = Delegate.Combine(existing, callback);
            }
            else {
                Events[type] = callback;
            }
        }

        public static void Unsubscribe<T>(Action<T> callback) {
            var type = typeof(T);

            if (Events.TryGetValue(type, out var existing)) {
                var current = Delegate.Remove(existing, callback);

                if (current == null)
                    Events.Remove(type);
                else
                    Events[type] = current;
            }
        }

        public static void Clear() {
            Events = new Dictionary<Type, Delegate>();
        }

    }
}