using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine.UI;


public class Deck3DManagerScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> deck_cards = new List<GameObject>();
    [SerializeField] private List<GameObject> hand_cards = new List<GameObject>();
    [SerializeField] private List<GameObject> board_cards = new List<GameObject>();

    public GameObject card_prefab;

    public Transform deck_spawn_point;
    public Transform hand_spawn_point;
    public Transform board_spawn_point;

    public TextureManagerScript textureManager;

    public Text debugobj;

    void Start()
    {
        Spawn_Deck(20);
        MoveCardToHand("5");
        MoveCardToHand("9");
        MoveCardToHand("12");
        MoveCardToHand("3");
        MoveCardToHand("21");
    }

    public void Spawn_Deck(int quantity)
    {
        Vector3 thickness = new Vector3(0, 0.005f, 0);
        Vector3 previous = new Vector3(0, 0, 0);
        Delete_Deck(deck_cards);
        for (int i = 0; i < quantity; i++, previous += thickness)
        {
            GameObject card = Instantiate(card_prefab, deck_spawn_point.position + previous, deck_spawn_point.rotation);
            card.transform.SetParent(this.gameObject.transform);
            deck_cards.Add(card);
        }
    }
    public void Delete_Deck(List<GameObject> list_of_cards)
    {
        foreach (GameObject card in list_of_cards)
        {
            #if UNITY_EDITOR
            DestroyImmediate(card);
            #else
            Destroy(card);
            #endif
        }

        deck_cards.Clear();
    }

    public void MoveCardToHand(string id)
    {
        GameObject picked_card = deck_cards[deck_cards.Count - 1]; // Pick the last card
        deck_cards.RemoveAt(deck_cards.Count - 1);                  // Remove the last card
        hand_cards.Add(picked_card);                                // Add it to hand_cards
        AddaptAllCardsPositions();
        try
        {
            textureManager.Texture_Card(picked_card, id);
        }
        catch (System.Exception ex) // Catching a general exception
        {
            debugobj.text = $"Error: {ex.Message}";
        }
    }

    [ContextMenu("Move Card To Board")]
    public void MoveCardToBoard()
    {
        string id = "003";
        GameObject picked_Card = hand_cards.Find(card => card.name.StartsWith(id + "_"));
        hand_cards.Remove(picked_Card);
        board_cards.Add(picked_Card);
        picked_Card.transform.position = board_spawn_point.transform.position;
        picked_Card.transform.rotation = board_spawn_point.transform.rotation;
        AddaptAllCardsPositions();
    }

    public void AddaptAllCardsPositions()
    {
        int card_count = hand_cards.Count;
        int loop_count = 0;
        float card_spacing = 0.3f;
        float curve_height = 0.5f;
        foreach (GameObject card in hand_cards)
        {
            float x_offset = (loop_count - (card_count - 1) / 2f) * card_spacing;
            float a = -curve_height / Mathf.Pow((card_count - 1) / 2f, 2); // Calculate steepness
            float y_offset = a * Mathf.Pow(x_offset, 2) + curve_height;
            if (float.IsNaN(y_offset) || float.IsInfinity(y_offset))
                y_offset = 0;
            card.transform.position = new Vector3(
                hand_spawn_point.position.x + x_offset,
                hand_spawn_point.position.y + y_offset,
                hand_spawn_point.position.z - (loop_count - card_count / 2) * 0.001f
            );
            card.transform.rotation = hand_spawn_point.rotation;

            loop_count++;
        }
    }

    [ContextMenu("Clear All")]
    private void Clear_All()
    {
        Delete_Deck(deck_cards);
        Delete_Deck(hand_cards);
        Delete_Deck(board_cards);
        deck_cards = new List<GameObject>();
        hand_cards = new List<GameObject>();
        board_cards = new List<GameObject>();
    }
    [ContextMenu("Test Spawn Deck")]
    private void Test_Spawn_Deck()
    {
        Spawn_Deck(20);
    }

    public void Pick_Cards(string jsonString)
    {
        jsonString = Regex.Replace(jsonString, @"[^0-9,]", "");
        string[] card_ids = jsonString.Split(new string[] { "," }, StringSplitOptions.None);
        foreach (string id in card_ids)
            MoveCardToHand(id);
    }

}

