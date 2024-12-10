using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSocketScript : MonoBehaviour
{
    private SocketClient SocketScript;
    public Text DebugOutput;
    public Deck3DManagerScript Deck3DManager;

    void Start()
    {
        SocketScript = FindObjectOfType<SocketClient>();
        SocketScript.SelfRegisterGameScript(this);
        StartCoroutine(StartTimer());
    }

    //function that wait 5 seconds before telling the server that we're ready(via the socket script)
    private System.Collections.IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(5f);

        SocketScript.Scene_is_ready();
    }

    public void Show(string output)
    {
        DebugOutput.text = output;
        Deck3DManager.Spawn_Deck(20);
        Deck3DManager.Pick_Cards(output);
    }

}
