using UnityEngine;
using SocketIOClient;

public class SocketClient : MonoBehaviour
{
    private SocketIOUnity socket;
    private string serverUrl = "https://lemming-national-anemone.ngrok-free.app/";
    public bool connected = false;

    void Start()
    {
        socket = new SocketIOUnity(serverUrl, new SocketIOOptions
        {
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        socket.OnConnected += (sender, e) =>
        {
            connected = true;
            Debug.Log("Connected to server!");
            socket.EmitAsync("message", "Hello from Unity!");
        };

        socket.On("response", (data) =>
        {
            Debug.Log("Received response from server: " + data);
        });

        socket.OnDisconnected += (sender, e) =>
        {
            connected = false;
            Debug.Log("Disconnected from server");
        };

        socket.ConnectAsync();
    }

    private void OnDestroy()
    {
        if (socket != null)
            socket.DisconnectAsync();
    }
}
