using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PupitreCardInteraction : MonoBehaviour
{
    public GameSocketScript GameSocketScript;
    public PupitreScript Pupitre_Script;
    public CardTypesManagerScript TypeManagerScript;

    public Text debugobj;

    void Validate_Card(string card_id, int card_position, string phase)
    {
        GameSocketScript.SocketScript.Validate_Card(card_id, card_position, phase);
    }

    public void Test1()
    {
        debugobj.text = "Test1";
        foreach(GameObject card in Pupitre_Script.hand_cards)
        {
            string card_id = card.name.Substring(0, 3);
            string card_type = TypeManagerScript.GetType1FromID(card_id);
            if(card_type == "Equipement")
            {
                Debug.Log($"Test1: Equipement card found: {card_id}");
                MoveCardToBoard(card_id, 0);
                return;
            }
        }
        Debug.Log("Test1: No Equipement card found");
        debugobj.text = "No Equipement card found";
    }
    public void Test2()
    {
        debugobj.text = "Test2";
        foreach(GameObject card in Pupitre_Script.hand_cards)
        {
            string card_id = card.name.Substring(0, 3);
            string card_type = TypeManagerScript.GetType1FromID(card_id);
            if(card_type != "Equipement")
            {
                Debug.Log($"Test2: Non Equipement card found: {card_id}");
                MoveCardToBoard(card_id, 3);
                return;
            }
        }
        Debug.Log("Test2: No wrong card found");
        debugobj.text = "No wrong card found";
    }
    public void Test3()
    {
        debugobj.text = "Test3";
        foreach(GameObject card in Pupitre_Script.hand_cards)
        {
            string card_id = card.name.Substring(0, 3);
            string card_type = TypeManagerScript.GetType1FromID(card_id);
            if(card_type == "Trap")
            {
                Debug.Log($"Test3: Trap card found: {card_id}");
                MoveCardToBoard(card_id, 1);
                return;
            }
        }
        Debug.Log("Test3: No Trap card found");
        debugobj.text = "No Trap card found";
    }
    public void Test4()
    {
        debugobj.text = "Test4";
        foreach(GameObject card in Pupitre_Script.hand_cards)
        {
            string card_id = card.name.Substring(0, 3);
            string card_type = TypeManagerScript.GetType1FromID(card_id);
            if(card_type != "Trap")
            {
                Debug.Log($"Test4: Non Trap card found: {card_id}");
                MoveCardToBoard(card_id, 4);
                return;
            }
        }
        Debug.Log("Test4: No wrong card found");
        debugobj.text = "No wrong card found";
    }
    public void Test5()
    {
        debugobj.text = "Test5";
        foreach(GameObject card in Pupitre_Script.hand_cards)
        {
            string card_id = card.name.Substring(0, 3);
            string card_type = TypeManagerScript.GetType1FromID(card_id);
            if(card_type == "Action")
            {
                Debug.Log($"Test5: Action card found: {card_id}");
                MoveCardToBoard(card_id, 6);
                return;
            }
        }
        Debug.Log("Test5: No Action card found");
        debugobj.text = "No Action card found";
    }
    public void Test6()
    {
        debugobj.text = "Test6";
        foreach(GameObject card in Pupitre_Script.hand_cards)
        {
            string card_id = card.name.Substring(0, 3);
            string card_type = TypeManagerScript.GetType1FromID(card_id);
            if(card_type != "Action")
            {
                Debug.Log($"Test6: Non Action card found: {card_id}");
                MoveCardToBoard(card_id, 2);
                return;
            }
        }
        Debug.Log("Test6: No wrong card found");
        debugobj.text = "No wrong card found";
    }

    private bool IsSlotAvailable(int slot_index)
    {
        return (Pupitre_Script.board_cards[slot_index] == null);
    }

    private bool can_move_card(string card_id, int slot_index)
    {
        if(!IsSlotAvailable(slot_index))
            return false;
        string type = TypeManagerScript.GetType1FromID(card_id);
        string phase = GameSocketScript.current_phase;

        if (phase == "Action" && slot_index == 6 && type == "Action") //card action can only be played in the action slot an
            return true;
        if (phase != "Preparation" || slot_index == 6 ||type == "Action") //card action should work in condition before
            return false;                                                   //only non action cards on phase preparation should be left
        if (slot_index < 3 && type == "Equipement") //equipement cards can only be played in the first 3 slots
            return true;
        if (slot_index > 2 && type == "Trap")    //trap cards can only be played in the last 3 slots
            return true;

        return false;   //rest is invalid
    }

    private void MoveCardToBoard(string card_id, int slot_index)
    {
        if(can_move_card(card_id, slot_index))
        {
            Pupitre_Script.MoveCardToBoard(card_id, slot_index);
            Validate_Card(card_id, slot_index, GameSocketScript.current_phase);
        }
        else
            Debug.Log($"Slot {slot_index} is not available");
    }
}
