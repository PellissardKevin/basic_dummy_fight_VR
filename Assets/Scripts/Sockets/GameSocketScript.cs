using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.Text.RegularExpressions;
using System;
using TMPro;


public class GameSocketScript : MonoBehaviour
{
    [HideInInspector]public SocketClient SocketScript;
    public UIDummyDamageDisplay DummyDisplay;
    public PupitreScript PupitreScript;
    public TimerScript timerScript;
    public WinCondition WinConditionScript;
    public HealthManagerScript HealthManager;
    public DummyController DummyControllerPlayer;
    public DummyController DummyControllerOponent;

    public FieldManager FieldManagerPlayer;
    public FieldManager FieldManagerOponent;

    public TMP_Text PhaseText;
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
        Debug.Log($"Picking card: {id}");
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

    public void next_phase(string my_cards, string oponent_cards, string cards_to_reveal, string own_reveal, string phase, string timer, string game_status, string your_damage, string oponent_damage, string discarded_cards, int count_discarded)
    {
        Debug.Log($"Next {my_cards}, {oponent_cards}, {phase}, {timer}");
        timerScript.ResetTimer(float.Parse(timer, CultureInfo.InvariantCulture));
        Debug.Log($"Cards to reveal: {cards_to_reveal}");

        current_phase = phase;
        PhaseText.text = phase;

        if (phase == "Preparation" || phase == "Action" || phase == "Discard")
            TurnEffect.color = new Color(0, 1, 0);
        else
            TurnEffect.color = new Color(1, 0, 0);

        if (phase == "Draw")
        {
            Pick_Cards(my_cards);
            Discard(discarded_cards, count_discarded);
        }
        else if (phase == "Reveal")
        {
            RevealCards(cards_to_reveal, false);
            RevealCards(own_reveal, true);
        }
        else if (phase == "Action")
        {
            PupitreScript.ReduceCardsTimer();
            FieldManagerPlayer.ReduceCardsTimer();
            FieldManagerOponent.ReduceCardsTimer();
        }
        else if (phase == "Resolve")
        {
            DummyDisplay.CreateFloatingText("health", int.Parse(your_damage), true);
            DummyDisplay.CreateFloatingText("health", int.Parse(oponent_damage), false);
            HealthManager.change_health(1, -int.Parse(your_damage));
            HealthManager.change_health(2, -int.Parse(oponent_damage));
            DummyControllerPlayer.PlayAnimation();
            DummyControllerOponent.PlayAnimation();
            WinConditionScript.test_victory(game_status);

        }
        else if (phase == "Discard")
        {
            PupitreScript.TrashActionCard();

        }

        phase_ended = false;
    }

    public void Discard(string discarded_cards, int count_discarded)
    {
        var card_ids = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(discarded_cards);

        foreach (string id in card_ids)
        {
            string padded_id = id.PadLeft(3, '0');
            Debug.Log($"Discarding card: {id} {padded_id}");
            PupitreScript.Discard(padded_id);
            FieldManagerPlayer.Discard(padded_id);
        }

        Debug.Log($"Discarded {count_discarded} cards {discarded_cards}");
    }

    public void RevealCards(string cards_to_reveal, bool isPlayer)
    {
        //format "[[\"5\", 2, [[\"type\", 2, \"target\" ], [\"type\", 2, \"target\" ]]], [\"3\", 5, [[\"type\", 2, \"target\" ]]], [\"1\", 3, []]]"
        var listOfCards = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<object>>>(cards_to_reveal);
        foreach (var card in listOfCards)
        {
            string card_id = card[0].ToString();
            int card_slot = Convert.ToInt32(card[1]);
            Newtonsoft.Json.Linq.JArray effects = card[2] as Newtonsoft.Json.Linq.JArray;

            foreach (var effect in effects)
            {
                var effectList = effect.ToObject<List<object>>();
                string type = effectList[0].ToString();
                int value = Convert.ToInt32(effectList[1]);
                string target = effectList[2].ToString();
                //Debug.Log($"Type: {type}, Value: {value}, Target: {target}");
                DummyDisplay.CreateFloatingText(type, value, target == "self");
            }
            Reveal_Card(card_id, card_slot, isPlayer);
            //Debug.Log($"Revealing card: {card_id} in slot {card_slot} with effects: {effects}");
        }
    }

    public void Pick_Cards(string jsonString)
    {
        var card_ids = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jsonString);

        foreach (string id in card_ids)
            Pick_Card(id);
    }

    public void end_Phase()
    {
        if (!phase_ended)
        {
            phase_ended = true;
            //Debug.Log($"Validating Phase {current_phase}");
            SocketScript.Validate_Cards(new List<string>(), current_phase);
        }
    }

    public void phase_validation_accepted()
    {
        //Debug.Log("Accepted");
    }

    public void phase_validation_denied()
    {
        //Debug.Log("Denied");
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
        FieldManagerPlayer.Spawn_Deck(Q1);
        FieldManagerOponent.Spawn_Deck(Q2);
        Debug.Log($"set deck: {Q1}, {Q2}");
    }

    [ContextMenu("Reveal Cards")]
    public void test_reveal2()
    {
        //RevealCards2("[[\"5\", 2, [\"effect1\", \"effect2\"]], [\"3\", 5, [\"effect1\"]], [\"1\", 3, []]]");
        RevealCards("[[\"5\", 2, [[\"type\", 2, \"target\" ], [\"type\", 2, \"target\" ]]], [\"3\", 5, [[\"type\", 2, \"target\" ]]], [\"1\", 3, []]]", true);
    }



}
