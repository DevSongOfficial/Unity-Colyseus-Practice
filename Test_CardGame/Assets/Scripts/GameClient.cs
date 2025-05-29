using System;
using System.Collections.Generic;
using UnityEngine;
using Colyseus;
using Colyseus.Schema;


// Base message wrapper for sending data from Unity to the server.
// When receiving data from the server, use Dictionary<string, object>.
public class Message 
{                    
    public string chatMessage;
}

// A central place to define all message types used between Unity and Node.js.
// These are for avoiding typos and improving readability. (Temp)
public static class MessageTypes
{
    //this.onMessage(Message Types, (client, data) => { ... }));
    public static readonly string Message = "message"; // == data.(given message)
    public static readonly string Sender = "sender";   // == client.sessionId

    // Message Types
    public static readonly string PlayerJoined = "player_joined";
    public static readonly string Chat = "chat_message";
}

public class GameState : Schema
{
// Game state synchronized between client and server.
// Add fields here if you want real-time sync via Colyseus
}

public class GameClient : MonoBehaviour
{
    private ColyseusClient client; // Client Object
    private ColyseusRoom<GameState> room; // ColyseusRoom: 서버의 특정 방에 연결된 객체. <T>는 동기화 할 Schema

    // Events
    public event Action<Dictionary<string, object>> OnPlayerJoined;
    public event Action<ColyseusRoom<GameState>> OnRoomReady;

    private async void Awake()
    {
        client = new ColyseusClient("ws://localhost:2567");
        room = await client.JoinOrCreate<GameState>("gameRoom");
        OnRoomReady?.Invoke(room);

        // Register message handler for when a player joins the room
        room.RegisterMessageHandler(MessageTypes.PlayerJoined, message => OnPlayerJoined.Invoke(message));
    }
}