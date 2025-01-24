using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PupitreScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> deck_cards = new List<GameObject>();
    [SerializeField] public List<GameObject> hand_cards = new List<GameObject>();
    [SerializeField] public List<GameObject> trash_cards = new List<GameObject>();
    [SerializeField] public GameObject[] board_cards = new GameObject[7];

    public Transform deck_spawn_point;
    public Transform hand_spawn_point;
    public Transform[] board_spawn_point = new Transform[7];
    public Transform board;
    public Transform trash;

    public GameObject card_prefab;

    public TextureManagerScript textureManager;

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
        Debug.Log($"Moving card {id} to slot {slot_index}");
        GameObject card_to_move = null;
        foreach (GameObject card in hand_cards)
            if (card != null && card.name.Substring(0, 3) == id)
                card_to_move = card;

        hand_cards.Remove(card_to_move);
        board_cards[slot_index] = card_to_move;

        card_to_move.transform.position = board_spawn_point[slot_index].position;
        AddaptAllCardsPositions();
    }

    public void AddaptAllCardsPositions()
    {
        int card_count = hand_cards.Count;
        int loop_count = 0;
        float card_spacing = 0.35f / 2.5f;

        foreach (GameObject card in hand_cards)
        {
            float x_offset = (loop_count - (card_count - 1) / 2f) * card_spacing;
            card.transform.localPosition = new Vector3(
                hand_spawn_point.localPosition.x + x_offset,
                hand_spawn_point.localPosition.y ,
                hand_spawn_point.localPosition.z - 0.001f * loop_count
            );
            // Set the card's rotation relative to the board
            card.transform.localRotation = Quaternion.identity; // Aligns with the parent
            card.transform.rotation = board.rotation;

            loop_count++;
        }
    }

    public void MoveCardToBoard(GameObject card, int slot_index)
    {
        hand_cards.Remove(card);
        if (slot_index != 8)
        {
            board_cards[slot_index] = card;
            card.transform.position = board_spawn_point[slot_index].position;
            card.transform.rotation = board_spawn_point[slot_index].rotation;
        }
        else
            MoveToTrash(card);
        AddaptAllCardsPositions();
    }

    public void MoveToTrash(GameObject card)
    {
        Vector3 thickness = new Vector3(0, 0, -0.002f) * board.transform.lossyScale.z; // Offset for stacking in local space
        card.transform.position = trash.position + thickness * trash_cards.Count;
        trash_cards.Add(card);
        AddaptAllCardsPositions();
    }

    [ContextMenu("Reduce card timer")]
    public void ReduceCardsTimer()
    {
        for (int i = board_cards.Length - 1; i >= 0; i--) // Iterate backwards to safely remove
        {
            GameObject card = board_cards[i];
            if (card == null)
                continue;

            TMP_Text timerText = card.transform.Find("TimerCanvas/TimerText").GetComponent<TMP_Text>();
            int timer = int.Parse(timerText.text);
            timer--;

            if (timer > 0)
                timerText.text = timer.ToString();
            else
            {
                MoveToTrash(card);
                board_cards[i] = null; // Nullify the reference in the list
            }
        }
    }

}
