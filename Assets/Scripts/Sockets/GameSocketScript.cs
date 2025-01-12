using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.Text.RegularExpressions;
using System;


public class GameSocketScript : MonoBehaviour
{
    [HideInInspector]public SocketClient SocketScript;
    public Deck3DManagerScript Deck3DManager;
    public PupitreScript PupitreScript;
    public TimerScript timerScript;

    public FieldManager FieldManagerPlayer;
    public FieldManager FieldManagerOponent;

    public PC_Card_Interaction Card_Interaction_Script;

    public Text DebugOutput;
    public Text debugobj2;

    public Text PhaseText;
    public GameObject opponent_player;
    public string current_phase = "Draw";

    public Material TurnEffect;

    private bool phase_ended = false;

    void Start()
    {
        SocketScript = FindObjectOfType<SocketClient>();
        SocketScript.SelfRegisterGameScript(this);
        StartCoroutine(StartTimer());
    }

    //function that wait 5 seconds before telling the server that we're ready(via the socket script)
    private System.Collections.IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(2f);

        SocketScript.Scene_is_ready();
    }

    public void Pick_Card(string id)
    {
        Deck3DManager.MoveCardToHand(id);
        PupitreScript.MoveCardToHand(id);
        FieldManagerPlayer.MoveCardToHand(id);
        FieldManagerOponent.MoveCardToHand(id);
    }
    public void Reveal_Card(string id, int slot, bool isPlayer)
    {
        if (isPlayer)
            FieldManagerPlayer.PlaceCardOnBoard(id, slot);
        else
            FieldManagerOponent.PlaceCardOnBoard(id, slot);
    }

    public void next_phase(string my_cards, string oponent_cards, string cards_to_reveal, string phase, string timer)
    {
        Debug.Log($"Next {my_cards}, {oponent_cards}, {phase}, {timer}");
        timerScript.ResetTimer(float.Parse(timer, CultureInfo.InvariantCulture));

        current_phase = phase;
        PhaseText.text = phase;

        if (phase == "Preparation")
            TurnEffect.color = new Color(0, 1, 0);
        else
            TurnEffect.color = new Color(1, 0, 0);

        if (phase == "Draw")
            Pick_Cards(my_cards);
        if (phase == "Reveal")
            RevealCards(cards_to_reveal);

        debugobj2.text = $"Phase: {phase}";
        phase_ended = false;
    }

    public void RevealCards(string cards_to_reveal)
    {
        //format is: [[card_id, card_slot], [card_id, card_slot], ...]
        if (cards_to_reveal == "[]")
        {
            Debug.Log("No cards to reveal");
            return;
        }
        Debug.Log($"Revealing cards: {cards_to_reveal}");

        cards_to_reveal = cards_to_reveal.Replace("],", ";").Replace("[", "").Replace("]", "").Replace(" ", "").Replace("\"", ""); // Replace "]," with ";", then remove "[" and "]" and "
        string[] id_slot_pairs = cards_to_reveal.Split(';');
        Debug.Log(id_slot_pairs);
        foreach (string id_slot_pair in id_slot_pairs)
        {
            string[] id_slot = id_slot_pair.Split(",");
            Debug.Log(id_slot);
            string id = id_slot[0];
            int slot = int.Parse(id_slot[1]);
            Debug.Log($"Revealing card: {id} in slot {slot}");
            FieldManagerOponent.PlaceCardOnBoard(id, slot);
        }

        Debug.Log($"Revealing cards: {cards_to_reveal}");
    }

    [ContextMenu("Reveal Cards")]
    public void test_reveal()
    {
        RevealCards("[[\"5\", 2], [\"3\", 5], [\"1\", 3]]");
    }

    public void Pick_Cards(string jsonString)
    {
        jsonString = Regex.Replace(jsonString, @"[^0-9,]", "");
        string[] card_ids = jsonString.Split(new string[] { "," }, StringSplitOptions.None);
        foreach (string id in card_ids)
        {
            Pick_Card(id);
        }
    }

    public void end_Phase()
    {
        if (!phase_ended)
        {
            phase_ended = true;
            Debug.Log($"Validating Phase {current_phase}");
            SocketScript.Validate_Cards(new List<string>(), current_phase);
        }
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

        // Set the opponent_player's position
        opponent_player.transform.position = new Vector3(-x, y, -z);
        opponent_player.transform.rotation = new Quaternion(rx, ry, rz, rw);

    }

    public void send_player_position(string position)
    {
        SocketScript.send_player_position(position);
    }

    public void set_deck(string myDeckQuantity, string oponentDeckQuantity)
    {
        int Q1 = int.Parse(myDeckQuantity);
        int Q2 = int.Parse(oponentDeckQuantity);

        PupitreScript.Spawn_Deck(Q1);
        Deck3DManager.Spawn_Deck(Q1);
        FieldManagerPlayer.Spawn_Deck(Q1);
        FieldManagerOponent.Spawn_Deck(Q2);
        Debug.Log($"set deck: {Q1}, {Q2}");
    }

}
