using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSocketScript : MonoBehaviour
{
    private SocketClient SocketScript;
    public Text DebugOutput;
    public Deck3DManagerScript Deck3DManager;
    public PC_Card_Interaction Card_Interaction_Script;

    public Text debugobj2;
    public GameObject opponent_player;
    public GameObject opponent_player2;
    public string current_phase = "Draw";

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

    public void Validate_Cards()
    {
        List<string> card_Ids = new List<string>();

        foreach (GameObject card in Card_Interaction_Script.cardList)
        {

            string cardName = card.name.Substring(0, 3); // Get the first 3 characters of the card's name (ID)
            string strippedName = cardName.TrimStart('0'); // Strip leading zeros

            card_Ids.Add(strippedName);
            Debug.Log(strippedName);
        }
        SocketScript.Validate_Cards(card_Ids, current_phase);
    }

    public void next_phase(string my_cards, string oponent_cards, string phase, string timer)
    {
        Debug.Log($"Next {my_cards}, {oponent_cards}, {phase}, {timer}");
        current_phase = phase;
        debugobj2.text = $"Phase: {phase}";
    }

    public void phase_validation_accepted()
    {
        Debug.Log("Accepted");
    }

    public void phase_validation_denied()
    {
        Debug.Log("Denied");
    }

    public void set_player_position(string transformString)
    {

        string[] transformParts = transformString.Split(':');
        string[] positionParts = transformParts[0].Split('_');
        string[] rotationParts = transformParts[1].Split('_');

        // Ensure the string has exactly 3 parts
        if (positionParts.Length != 3 || rotationParts.Length != 4)
        {
            Debug.LogError($"Invalid position format. Expected 'x_y_z:rx_ry_rz'. {transformString} {positionParts} {positionParts.Length}");
            return;
        }

        // Parse the string components to floats
        float x = float.Parse(positionParts[0]);
        float y = float.Parse(positionParts[1]);
        float z = float.Parse(positionParts[2]);
        float rx = float.Parse(rotationParts[0]);
        float ry = float.Parse(rotationParts[1]);
        float rz = float.Parse(rotationParts[2]);
        float rw = float.Parse(rotationParts[3]);

        Debug.Log($"Recieving: {x} {y} {z} {rx} {ry} {rz} {rw}");
        // Set the opponent_player's position
        opponent_player.transform.position = new Vector3(-x, y, -z);
        opponent_player.transform.rotation = new Quaternion(rx, ry, rz, rw);

        //opponent_player2.transform.position = new Vector3(-x + 1, y, -z);
        //opponent_player2.transform.rotation = new Quaternion(rx, -ry, -rz, rw);

    }

    public void send_player_position(string position)
    {
        SocketScript.send_player_position(position);
    }

}
