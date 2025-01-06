using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PupitreCardInteraction : MonoBehaviour
{
    public GameSocketScript GameSocketScript;

    void Validate_Card(int card_id, int card_position, string phase)
    {
        GameSocketScript.SocketScript.Validate_Card(card_id, card_position, phase);
    }

    public void Test1()
    {
        GameSocketScript.SocketScript.Validate_Card(1, 0, "Draw");
    }
    public void Test2()
    {
        GameSocketScript.SocketScript.Validate_Card(2, 0, "Draw");
    }
    public void Test3()
    {
        GameSocketScript.SocketScript.Validate_Card(3, 0, "Draw");
    }
    public void Test4()
    {
        GameSocketScript.SocketScript.Validate_Card(4, 0, "Draw");
    }
    public void Test5()
    {
        GameSocketScript.SocketScript.Validate_Card(5, 0, "Draw");
    }
    public void Test6()
    {
        GameSocketScript.SocketScript.Validate_Card(6, 0, "Draw");
    }
}
