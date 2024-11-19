using UnityEngine;
using UnityEngine.UI;
using SocketIOClient;
using System.Collections.Generic;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Debug = UnityEngine.Debug;


public class SocketClient : MonoBehaviour
{
    private SocketIOUnity socket;
    private string serverUrl = "https://lemming-national-anemone.ngrok-free.app/";
    public bool connected = false;
    public string state = "disconnected";

    public Text statusText; // UI element to display state
    public Text messageText; // UI element to display messages
    public GameObject startQueueButton; // Button for starting queue
    public GameObject yesButton; // Button for "Yes" response
    public GameObject noButton; // Button for "No" response
    public GameObject endButton; // Button for ending match

    //queue used to do unity stuff in the main thread
    ConcurrentQueue<Action> functionQueue = new ConcurrentQueue<Action>();

    //types:
    //sender:   SocketIOUnity
    //e:        System.EventArgs
    //data:     SocketIOClient.SocketIOResponse

    void Start()
    {
        UpdateUI(state);
        socket = new SocketIOUnity(serverUrl, new SocketIOOptions
        {
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        socket.OnConnected += (sender, e) => { ErrorCatcher(() => connect()); };
        socket.OnDisconnected += (sender, e) => { ErrorCatcher(() => disconnect()); };
        socket.OnError += (sender, e) => { ErrorCatcher(() => error(e)); };

        socket.On("status", (data) => { ErrorCatcher(() => socket_status_update(data)); });
        socket.On("prompt_match", (data) => { ErrorCatcher(() => prompt_match(data)); });
        socket.On("server_recieved_match_response", (data) => { ErrorCatcher(() => server_recieved_match_response()); });
        socket.On("response", (data) =>
        {
            Debug.Log("Received response from server: " + data);
        });

        socket.ConnectAsync();
        Debug.Log("Starting...");
    }

    private void Update()
    {
        //Do unity stuff here in the main thread
        //(queue it with sockets and dequeue it here)
        if (functionQueue.TryDequeue(out Action action))
            action?.Invoke();
    }

    private void connect()
    {
        connected = true;
        Debug.Log("Connected to server!");
    }

    private void disconnect()
    {
        connected = false;
        Debug.Log("Disconnected from server");
    }

    private void error(string e)
    {
        Debug.LogError("Socket.IO Error: " + e);
    }

    private void socket_status_update(SocketIOClient.SocketIOResponse data)
    {
        state = parse_response(data, "state");
        Debug.Log($"New state: {state}");
        functionQueue.Enqueue(() => statusText.text = "State: " + state );
        Debug.Log("before status text");
        Debug.Log("before parse");

        functionQueue.Enqueue(() => UpdateUI(state) );

        Debug.Log("after parse");
    }

    private void prompt_match(SocketIOClient.SocketIOResponse data)
    {
        Debug.Log("Prompt received: " + data);
        string message = parse_response(data, "message");

        functionQueue.Enqueue(() =>
        {
            messageText.text = message;
            messageText.gameObject.SetActive(true);
            yesButton.SetActive(true);
            noButton.SetActive(true);
        });
    }

    private void server_recieved_match_response()
    {
        Debug.Log("Server acknowledged match response");

        functionQueue.Enqueue(() =>
        {
            yesButton.SetActive(false);
            noButton.SetActive(false);
        });
    }

    private void UpdateUI(string state)
    {
        // Reset all UI elements
        messageText.gameObject.SetActive(false);
        startQueueButton.SetActive(false);
        yesButton.SetActive(false);
        noButton.SetActive(false);
        endButton.SetActive(false);

        // Show relevant UI elements based on state
        if (state == "connected")
        {
            Debug.Log("activating button start");
            startQueueButton.SetActive(true);
        }
        if (state == "in_match")
            endButton.SetActive(true);
    }
    public void StartQueue()
    {
        if (connected)
        {
            Debug.Log("Starting queue...");
            socket.EmitAsync("start_queue");
        }
    }
    private string parse_response(SocketIOClient.SocketIOResponse data, string key)
    {
        var jsonObject = data.GetValue<Dictionary<string, string>>();
        if (!jsonObject.ContainsKey(key))
        {
            Debug.LogError($"Key {key} not found in the JSON object.");
            return null;
        }
        return jsonObject[key];
    }

    public void RespondToMatch(bool response)
    {
        if (connected)
        {
            Debug.Log("Responding to match: " + response);
            socket.EmitAsync("match_response", new { response = response });
        }
    }


    public void EndMatch()
    {
        if (connected)
        {
            Debug.Log("Ending match...");
            socket.EmitAsync("end_match");
        }
    }

    private void OnDestroy()
    {
        if (socket != null)
            socket.DisconnectAsync();
    }

    private void ErrorCatcher(Action action)
    {
        try
        {
            action.Invoke();  // Execute the action
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error Catcher: in {action.Method.Name} - {ex.Message}\n{ex.StackTrace}");
        }
    }
}
