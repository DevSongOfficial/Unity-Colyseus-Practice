using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Colyseus;

public class ChatManager : MonoBehaviour
{
    [SerializeField] private GameClient client;
    private ColyseusRoom<GameState> room;

    [Header("UI")]
    [SerializeField] private TMP_InputField chatInputField;
    [SerializeField] private TextMeshProUGUI chatLogText;
    [SerializeField] private Button sendButton;

    private void Awake()
    {
        client.OnRoomReady += SetupRoomHandlers;
        client.OnPlayerJoined += PrintPlayerJoinMessage;
        sendButton.onClick.AddListener(SendChatMessageAsync);
    }

    private void SetupRoomHandlers(ColyseusRoom<GameState> room)
    {
        this.room= room;

        room.RegisterMessageHandler(MessageTypes.Chat, message =>
        {
            string log = $"[{message[MessageTypes.Sender]}]: {message[MessageTypes.Message]}";
            chatLogText.text += log + "\n";
        });
    }

    private void PrintPlayerJoinMessage(Dictionary<string, object> message)
    {
        chatLogText.text += $"{message[MessageTypes.Sender]} has joined the room\n";
    }

    private async void SendChatMessageAsync()
    {
        await room.Send(MessageTypes.Chat, new Message { chatMessage = chatInputField.text });
        chatInputField.text = string.Empty;
    }
}