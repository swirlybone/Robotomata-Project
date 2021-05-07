using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NotificationCenter
{
    private static NotificationCenter _instance;
    protected NotificationCenter() { }
    public static NotificationCenter Instance()
    {
        if (_instance == null)
        {
            _instance = new NotificationCenter();
        }

        return _instance;
    }

    private Dictionary<String, EventContainer> listeners = new Dictionary<String, EventContainer>();

    private class EventContainer
    {
        private event Action<Notification> Observer;
        public EventContainer()
        {
        }

        public void addObserver(Action<Notification> observer)
        {
            Observer += observer;
        }

        public void removeObserver(Action<Notification> observer)
        {
            Observer -= observer;
        }

        public void sendNotification(Notification notification)
        {
            Observer(notification);
        }

        public bool isEmpty()
        {
            return Observer == null;
        }


    }

    public static void AddObserver(String notificationName, Action<Notification> observer)
    {
        if (!Instance().listeners.ContainsKey(notificationName))
        {
            Instance().listeners[notificationName] = new EventContainer();
        }
        Instance().listeners[notificationName].addObserver(observer);
    }

    public static void RemoveObserver(String notificationName, Action<Notification> observer)
    {
        if (Instance().listeners.ContainsKey(notificationName))
        {
            Instance().listeners[notificationName].removeObserver(observer);
            if (Instance().listeners[notificationName].isEmpty())
            {
                Instance().listeners.Remove(notificationName);
            }
        }
    }
    public static void PostNotification(String name)
    {
        Notification notification = new Notification(name);
        if (Instance().listeners.ContainsKey(notification.Name))
        {
            Instance().listeners[notification.Name].sendNotification(notification);
        }
    }

    public static void PostNotification(String name, object thing)
    {
        Notification notification = new Notification(name, thing);
        if (Instance().listeners.ContainsKey(notification.Name))
        {
            Instance().listeners[notification.Name].sendNotification(notification);
        }
    }

    public static void PostNotification(String name, object thing, Dictionary<String, object> userInfo)
    {
        Notification notification = new Notification(name, thing, userInfo);
        if (Instance().listeners.ContainsKey(notification.Name))
        {
            Instance().listeners[notification.Name].sendNotification(notification);
        }
    }

    public static void Clear() 
    {
        Instance().listeners.Clear();
    }


}
public class Notification
{
    public string Name { get; set; }
    public object Object { get; set; }
    public Dictionary<String, object> UserInfo { get; set; }

    public Notification() : this("NotificationName") { }

    public Notification(String name) : this(name, null) { }

    public Notification(String name, object obj) : this(name, obj, null) { }

    public Notification(String name, object obj, Dictionary<String, object> userInfo) 
    {
        Name = name;
        Object = obj;
        UserInfo = userInfo;
    }
}