using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Event Center handling events with or without arguments.
/// </summary>
public class EventCenter
{
    public static Dictionary<Type, Action> eventSubscribers = new Dictionary<Type, Action>();
    public static Dictionary<Type, Delegate> eventSubscribersWithData = new Dictionary<Type, Delegate>();

    public delegate void CallbackWithData<T>(T data) where T : IEventWithData;

    public static void RegisterEvent<T>(Action callback) where T : IEvent
    {
        if (!eventSubscribers.ContainsKey(typeof(T)))
        {
            eventSubscribers.Add(typeof(T), callback);
        }
        else
        {
            eventSubscribers[typeof(T)] += callback;
        }
    }

    public static void RegisterEvent<T>(CallbackWithData<T> callback) where T : IEventWithData
    {
        if (!eventSubscribersWithData.ContainsKey(typeof(T)))
        {
            eventSubscribersWithData.Add(typeof(T), callback);
        }
        else
        {
            Delegate currentDelegate;
            if (eventSubscribersWithData.TryGetValue(typeof(T), out currentDelegate))
            {
                eventSubscribersWithData[typeof(T)] = Delegate.Combine(currentDelegate, callback);
            }
        }
    }

    public static void UnRegisterEvent<T>(Action callback) where T : IEvent
    {
        if (eventSubscribers.ContainsKey(typeof(T)))
        {
            eventSubscribers[typeof(T)] -= callback;
        }
    }

    public static void UnRegisterEvent<T>(CallbackWithData<T> callback) where T : IEventWithData
    {
        Delegate currentDelegate;
        if (eventSubscribersWithData.TryGetValue(typeof(T), out currentDelegate))
        {
            eventSubscribersWithData[typeof(T)] = Delegate.Remove(currentDelegate, callback);
        }
    }

    public static void PostEvent<T>() where T : IEvent
    {
        if (eventSubscribers.ContainsKey(typeof(T)))
        {
            eventSubscribers[typeof(T)]?.Invoke();
        }
    }

    public static void PostEvent<T>(T eventData) where T : IEventWithData
    {
        if (eventSubscribersWithData.ContainsKey(typeof(T)))
        {
            if (eventSubscribersWithData[typeof(T)] != null)
            {
                ((CallbackWithData<T>)eventSubscribersWithData[typeof(T)])?.Invoke(eventData);
            }
        }
    }
}
