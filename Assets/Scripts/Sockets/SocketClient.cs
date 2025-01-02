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
    public ChangeScene SceneChanger;

    //queue used to do unity stuff in the main thread
    ConcurrentQueue<Action> functionQueue = new ConcurrentQueue<Action>();

    public string serverUrl = "https://lemming-national-anemone.ngrok-free.app/";
    public bool connected = false;
    public string state = "disconnected";
    private AccessToSocketScript UiScript;
    private GameSocketScript GameScript;

    //types:
    //sender:   SocketIOUnity
    //e:        System.EventArgs
    //data:     SocketIOClient.SocketIOResponse

    void Start()
    {
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
        socket.On("response", (data) => { Debug.Log("Received response from server: " + data); });
        socket.On("game_login_response", (data) => { ErrorCatcher(() => game_login_response(data)); });
        socket.On("pick_cards", (data) => { ErrorCatcher(() => { HandlePickCards(data); }); });
        socket.On("next_phase", (data) => { ErrorCatcher(() => { next_phase(data); }); });
        socket.On("phase_validation_accepted", (data) => { ErrorCatcher(() => { phase_validation_accepted(data); }); });
        socket.On("phase_validation_denied", (data) => { ErrorCatcher(() => { phase_validation_denied(data); }); });
        socket.On("player_move", (data) => { ErrorCatcher(() => { set_player_position(data); }); });

        socket.ConnectAsync();
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
        functionQueue.Enqueue(() => { SceneChanger.BackToMenu(); });
        Debug.Log("Disconnected from server");
    }

    private void error(string e)
    {
        Debug.LogError("Socket.IO Error: " + e);
    }

    private void socket_status_update(SocketIOClient.SocketIOResponse data)
    {
        if (state == "in_match")
            functionQueue.Enqueue(() => { SceneChanger.BackToMenu(); });

        state = parse_response(data, "state");
        Debug.Log($"New state: {state}");
        functionQueue.Enqueue(() =>
        {
            if (state == "in_match")
                SceneChanger.SwitchToGame();
            else
                UiScript.UpdateState(state);
        });
    }

    private void prompt_match(SocketIOClient.SocketIOResponse data)
    {
        string message = parse_response(data, "message");

        functionQueue.Enqueue(() => { UiScript.prompt_match(message); });
    }

    private void server_recieved_match_response()
    {
        functionQueue.Enqueue(() => { UiScript.server_recieved_match_response(); });
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
        var jsonObject = data.GetValue<Dictionary<string, object>>();
        var message = jsonObject[key];
        return message?.ToString();
    }

    public void RespondToMatch(bool response)
    {
        if (connected)
            socket.EmitAsync("match_response", new { response = response });
    }

    public void LogIn(string username, string password)
    {
        if (connected)
            socket.EmitAsync("game_login", new { username = username, password = password });
    }


    public void EndMatch()
    {
        if (connected)
            socket.EmitAsync("end_match");
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

    public void SelfRegister(AccessToSocketScript scpt)
    {
        if (scpt != null)
        {
            UiScript = scpt;
            CallUpdateUI(state);
        }
    }
    public void SelfRegisterGameScript(GameSocketScript scpt)
    {
        if (scpt != null)
        {
            GameScript = scpt;
        }
    }

    private void CallUpdateUI(string state)
    {
        UiScript.UpdateUI(state);
    }

    private void game_login_response(SocketIOClient.SocketIOResponse data)
    {
        string user_id = parse_response(data, "user_id");
        if (user_id == null)
        {
            Debug.Log($"user_id: null");
            functionQueue.Enqueue(() => { UiScript.Wrong_Login(); });
        }
        else
        {
            Debug.Log($"user_id: {user_id}");
            functionQueue.Enqueue(() => { UiScript.Good_Login(); });
        }
    }

    public void Scene_is_ready()
    {
        if (connected)
            socket.EmitAsync("ready_to_start");
    }

    private void HandlePickCards(SocketIOClient.SocketIOResponse data)
    {
        string repr_of_cards = parse_response(data, "cards");
        functionQueue.Enqueue(() => { GameScript.Show(repr_of_cards); });
        Debug.Log(repr_of_cards);
    }

    public void Validate_Cards(List<string> card_Ids, string phase)
    {
        if (connected)
            socket.EmitAsync("phase_validation", new { cards = card_Ids, phase = phase });
    }

    private void next_phase(SocketIOClient.SocketIOResponse data)
    {
        string my_cards = parse_response(data, "your_cards");
        string oponent_cards = parse_response(data, "oponent_cards");
        string phase = parse_response(data, "phase");
        string timer = parse_response(data, "timer");

        functionQueue.Enqueue(() => { GameScript.next_phase(my_cards, oponent_cards, phase, timer); });
    }

    private void phase_validation_accepted(SocketIOClient.SocketIOResponse data)
    {
        functionQueue.Enqueue(() => { GameScript.phase_validation_accepted(); });
    }

    private void phase_validation_denied(SocketIOClient.SocketIOResponse data)
    {
        functionQueue.Enqueue(() => { GameScript.phase_validation_denied(); });
    }

    private void set_player_position(SocketIOClient.SocketIOResponse data)
    {
        string position = parse_response(data, "position");
        functionQueue.Enqueue(() => { GameScript.set_player_position(position); });
    }

    public void send_player_position(string position)
    {
        if (connected)
            socket.EmitAsync("player_move", new { position = position });
    }
}