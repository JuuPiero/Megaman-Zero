using System;
using System.Collections.Generic;
using UnityEngine;

// Base class cho event args
public class EventArgs
{
    // Có thể thêm properties chung nếu cần
}

// Ví dụ event args cụ thể
public class PlayerEventArgs : EventArgs
{
    public string PlayerName { get; set; }
    public int PlayerScore { get; set; }
}

public static class EventBus
{
    private static readonly Dictionary<string, Delegate> events = new();

    // Đăng ký event với tham số là EventArgs
    public static void On<T>(string eventName, Action<T> callback) where T : EventArgs
    {
        if (events.ContainsKey(eventName))
            events[eventName] = Delegate.Combine(events[eventName], callback);
        else
            events[eventName] = callback;
    }

    public static void Off<T>(string eventName, Action<T> callback) where T : EventArgs
    {
        if (events.ContainsKey(eventName))
        {
            events[eventName] = Delegate.Remove(events[eventName], callback);
            if (events[eventName] == null)
                events.Remove(eventName);
        }
    }

    public static void Emit<T>(string eventName, T args) where T : EventArgs
    {
        if (events.TryGetValue(eventName, out var delegateObj))
        {
            (delegateObj as Action<T>)?.Invoke(args);
            #if UNITY_EDITOR
            Debug.Log($"Raise Event: {eventName} with args: {args}");
            #endif
        }
    }

    // Vẫn hỗ trợ event không tham số
    public static void On(string eventName, Action callback)
    {
        if (events.ContainsKey(eventName))
            events[eventName] = Delegate.Combine(events[eventName], callback);
        else
            events[eventName] = callback;
    }

    public static void Off(string eventName, Action callback)
    {
        if (events.ContainsKey(eventName))
        {
            events[eventName] = Delegate.Remove(events[eventName], callback);
            if (events[eventName] == null)
                events.Remove(eventName);
        }
    }

    public static void Emit(string eventName)
    {
        if (events.TryGetValue(eventName, out var delegateObj))
        {
            (delegateObj as Action)?.Invoke();
            #if UNITY_EDITOR
            Debug.Log("Raise Event: " + eventName);
            #endif
        }
    }
}