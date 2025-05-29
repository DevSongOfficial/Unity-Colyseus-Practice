using Colyseus;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Extensions
{
    // Registers a callback for the specified message type.
    // OnMessage<Dictionary<string, object>> 작성 귀찮아서 만듦
    public static void RegisterMessageHandler(this ColyseusRoom<GameState> room, string messageType, Action<Dictionary<string, object>> handler)
    {
        room.OnMessage(messageType, handler);
    }
}