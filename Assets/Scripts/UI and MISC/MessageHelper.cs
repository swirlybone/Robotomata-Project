using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MessageHelper
{
    public static void Log(string message) { NotificationCenter.PostNotification("Log", message); }
    public static void Message(string message) { NotificationCenter.PostNotification("Message", message); }
}
