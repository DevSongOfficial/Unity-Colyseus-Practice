using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Colyseus;

public class ChatManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField chatInputField;
    [SerializeField] private TextMeshProUGUI chatLogText;
    [SerializeField] private Button sendButton;

    private void Awake()
    {
        GameClient.client.OnRoomReady += SetupRoomHandlers;
        GameClient.client.OnPlayerJoined += PrintPlayerJoinMessage;
        sendButton.onClick.AddListener(SendChatMessageAsync);
    }

    private void SetupRoomHandlers(ColyseusRoom<GameState> room)
    {
        // Print chat whenever taking a message from server
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
        await GameClient.client.SendMessageToServer(MessageTypes.Chat, new Message { chatMessage = chatInputField.text });
        chatInputField.text = string.Empty;
    }
}