using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Colyseus;
using Colyseus.Schema;
using System.Threading.Tasks;
using UnityEngine.Networking;


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
    public static GameClient client;

    private static ColyseusClient colyseusClient; // Client Object
    private static ColyseusRoom<GameState> room; // ColyseusRoom: 서버의 특정 방에 연결된 객체. <T>는 동기화 할 Schema

    // Events
    public event Action<Dictionary<string, object>> OnPlayerJoined;
    public event Action<ColyseusRoom<GameState>> OnRoomReady;

    [Header("Joining Room")]
    [SerializeField] private Button joinButtonPrefab;
    [SerializeField] private Transform roomsPanel;
    private static List<Button> joinButtonList = new List<Button>();


    private void Awake()
    {
        client = this;
        colyseusClient = new ColyseusClient("ws://localhost:2567");
    }

    public async void CreateRoomAsync()
    {
        room = await colyseusClient.Create<GameState>("gameRoom");
        OnRoomReady?.Invoke(room);

        // Register message handler for when a player joins the room
        room.RegisterMessageHandler(MessageTypes.PlayerJoined, message => OnPlayerJoined.Invoke(message));
    }

    private async void JoinRoom(string roomId)
    {
        room = await colyseusClient.JoinById<GameState>(roomId);
        OnRoomReady?.Invoke(room);

        room.RegisterMessageHandler(MessageTypes.PlayerJoined, message => OnPlayerJoined.Invoke(message));
    }

    public async void RefreshRoomsAsync()
    {
        await RefreshRoomList();
    }

    private async Task RefreshRoomList()
    {
        UnityWebRequest request = UnityWebRequest.Get("http://localhost:2567/rooms"); // Connected to gameServer.define("lobby", LobbyRoom);
        var operation = request.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("FAILED TO REFRESH: " + request.error);
        }
        else
        {
            string json = "{\"rooms\":" + request.downloadHandler.text + "}"; // 배열 -> 객체 형태로 
            RoomListWrapper wrapper = JsonUtility.FromJson<RoomListWrapper>(json);

          
            // Set rooms

            for (int i = joinButtonList.Count - 1; i >= 0; i--)
                Destroy(joinButtonList[i].gameObject);
            joinButtonList.Clear();

            foreach (var room in wrapper.rooms)
            {
                Debug.Log($"Room ID: {room.roomId}, Players: {room.clients}/{room.maxClients}");
                
                var button = GameObject.Instantiate(joinButtonPrefab);
                button.onClick.AddListener(() => JoinRoom(room.roomId));
                button.transform.SetParent(roomsPanel);

                joinButtonList.Add(button);
            }
        }
    }

    public async Task SendMessageToServer(string type, object message)
    {
        await room.Send(type, message);
    }
}