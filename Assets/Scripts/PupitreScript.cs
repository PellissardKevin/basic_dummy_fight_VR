using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PupitreScript : MonoBehaviour
{

    [SerializeField] private List<GameObject> deck_cards = new List<GameObject>();
    [SerializeField] public List<GameObject> hand_cards = new List<GameObject>();
    [SerializeField] public GameObject[] board_cards = new GameObject[7];

    public Transform deck_spawn_point;
    public Transform hand_spawn_point;
    public Transform[] board_spawn_point = new Transform[7];
    public Transform board;

    public GameObject card_prefab;

    public TextureManagerScript textureManager;

    public void DebugDraw()
    {
        Spawn_Deck(20);
        MoveCardToHand("5");
        MoveCardToHand("3");
        MoveCardToHand("2");
        MoveCardToHand("6");
        MoveCardToHand("7");
        MoveCardToHand("13");
        MoveCardToHand("21");
        MoveCardToHand("25");
    }

    public void Spawn_Deck(int quantity)
    {
        Vector3 thickness = new Vector3(0, 0, -0.002f) * board.transform.lossyScale.z; // Offset for stacking in local space
        Delete_Deck(deck_cards); // Clear the previous deck

        for (int i = 0; i < quantity; i++)
        {
            GameObject card = Instantiate(card_prefab, deck_spawn_point.position, deck_spawn_point.rotation);
            card.transform.SetParent(board.gameObject.transform);
            Vector3 offset = deck_spawn_point.gameObject.transform.rotation * (thickness * i);
            card.transform.position = deck_spawn_point.position + offset;
            card.transform.Rotate(0, 180, 0, Space.Self);
            card.transform.localScale = card.transform.lossyScale / 3;
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
            Debug.Log($"Error: {ex.Message}");
        }
    }

    public void MoveCardToBoard(string id, int slot_index)
    {
        GameObject card_to_move = null;
        foreach (GameObject card in hand_cards)
            if (card != null && card.name.Substring(0, 3) == id)
                card_to_move = card;

        hand_cards.Remove(card_to_move);
        board_cards[slot_index] = card_to_move;

        card_to_move.transform.position = board_spawn_point[slot_index].position;

    }

    public void AddaptAllCardsPositions()
    {
        int card_count = hand_cards.Count;
        int loop_count = 0;
        float card_spacing = 0.35f;
        foreach (GameObject card in hand_cards)
        {
            float x_offset = (loop_count - (card_count - 1) / 2f) * card_spacing;
            card.transform.position = new Vector3(
                hand_spawn_point.position.x + x_offset,
                hand_spawn_point.position.y,
                hand_spawn_point.position.z
            );
            card.transform.rotation = hand_spawn_point.rotation;

            loop_count++;
        }
    }

    public void MoveCardToBoard(GameObject card, int slot_index)
    {
        hand_cards.Remove(card);
        board_cards[slot_index] = card;
        card.transform.position = board_spawn_point[slot_index].position;
        card.transform.rotation = board_spawn_point[slot_index].rotation;
        AddaptAllCardsPositions();
    }

}
