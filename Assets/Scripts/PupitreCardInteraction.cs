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

    void Start()
    {
        StartCoroutine(DebugTest(4f));
    }
    private IEnumerator DebugTest(float delay)
    {
        // Wait for the specified delay in seconds
        yield return new WaitForSeconds(delay);

        if (Pupitre_Script == null)
        {
            Debug.LogError("Pupitre_Script is null");
            yield break;
        }
        if (Pupitre_Script.hand_cards == null)
        {
            Debug.LogError("Pupitre_Script.hand_cards is null");
            yield break;
        }
        int count = 0;

        string text = "";
        foreach(GameObject card in Pupitre_Script.hand_cards)
        {
            count += 1;
            string card_id = card.name.Substring(0, 3);
            string card_type = TypeManagerScript.GetType1FromID(card_id);
            text += $"{card_type}\n";
        }
        text += $"Total cards: {count}";
        debugobj.text = text;
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
                GameSocketScript.SocketScript.Validate_Card(card_id, 0, GameSocketScript.current_phase);
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
                GameSocketScript.SocketScript.Validate_Card(card_id, 0, GameSocketScript.current_phase);
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
                GameSocketScript.SocketScript.Validate_Card(card_id, 0, GameSocketScript.current_phase);
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
                GameSocketScript.SocketScript.Validate_Card(card_id, 0, GameSocketScript.current_phase);
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
                GameSocketScript.SocketScript.Validate_Card(card_id, 0, GameSocketScript.current_phase);
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
                GameSocketScript.SocketScript.Validate_Card(card_id, 0, GameSocketScript.current_phase);
                return;
            }
        }
        Debug.Log("Test6: No wrong card found");
        debugobj.text = "No wrong card found";
    }
}
