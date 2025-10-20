using System;
using System.Collections.Generic;

namespace Game
{
    public class GameEvent { }

    public static class EventManager
    {
        static readonly Dictionary<Type, Action<GameEvent>> s_Events = new Dictionary<Type, Action<GameEvent>>();

        static readonly Dictionary<Delegate, Action<GameEvent>> s_EventLookups = new Dictionary<Delegate, Action<GameEvent>>();

        public static void AddListener<T>(Action<T> listener) where T : GameEvent
        {
            Action<GameEvent> action = (e) => listener((T)e);
            s_EventLookups[listener] = action;

            if (s_Events.TryGetValue(typeof(T), out Action<GameEvent> internalAction))
                s_Events[typeof(T)] = internalAction += action;
            else
                s_Events[typeof(T)] = action;
        }

        public static void RemoveListener<T>(Action<T> listener) where T : GameEvent
        {
            if (s_EventLookups.TryGetValue(listener, out Action<GameEvent> action))
            {
                if (s_Events.TryGetValue(typeof(T), out Action<GameEvent> internalAction))
                {
                    internalAction -= action;
                    if (internalAction == null)
                        s_Events.Remove(typeof(T));
                    else
                        s_Events[typeof(T)] = internalAction;
                }
                s_EventLookups.Remove(listener);
            }
        }

        public static void Broadcast<T>(T gameEvent) where T : GameEvent
        {
            if (s_Events.TryGetValue(typeof(T), out Action<GameEvent> action))
            {
                action?.Invoke(gameEvent);
            }
        }

        public static void Clear()
        {
            s_Events.Clear();
            s_EventLookups.Clear();
        }
    }
}

