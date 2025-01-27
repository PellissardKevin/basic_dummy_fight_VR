using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FieldManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> deck_cards = new List<GameObject>();
    [SerializeField] private List<GameObject> hand_cards = new List<GameObject>();
    [SerializeField] private List<GameObject> trash_cards = new List<GameObject>();

    [SerializeField] private GameObject[] Board = new GameObject[6];

    public GameObject card_prefab;

    public bool isPlayer = true;

    public Transform deck_spawn_point;
    public Transform hand_spawn_point;
    public Transform trash_spawn_point;

    public TextureManagerScript textureManager;

    public void Spawn_Deck(int quantity)
    {
        Vector3 thickness = new Vector3(0, 0, -0.02f) * deck_spawn_point.lossyScale.z; // Offset for stacking in local space
        Delete_Deck(deck_cards); // Clear the previous deck

        for (int i = 0; i < quantity; i++)
        {
            GameObject card = Instantiate(card_prefab, deck_spawn_point.position, deck_spawn_point.rotation);
            card.transform.SetParent(gameObject.transform);
            Vector3 offset = (thickness * i);
            card.transform.position = deck_spawn_point.position + offset;
            card.transform.Rotate(0, 180, 0, Space.Self);
            card.transform.localScale = card.transform.lossyScale * 3;
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

    public void AddaptAllCardsPositions()
    {
        int card_count = hand_cards.Count;
        int loop_count = 0;
        float card_spacing = 1.5f;
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
                hand_spawn_point.position.z - (loop_count - card_count / 2) * 0.01f
            );
            card.transform.rotation = hand_spawn_point.rotation;

            loop_count++;
        }
    }

    public void PlaceCardOnBoard(string id, int slot_index)
    {
        id = id.PadLeft(3, '0');

        GameObject card_to_move = null;

        if (hand_cards.Count > 0)
            card_to_move = hand_cards[0];
        else
        {
            Debug.Log("Card not found in hand");
            return;
        }

        hand_cards.Remove(card_to_move);
        AddaptAllCardsPositions();

        Debug.Log($"Placing card {id} on slot {slot_index}");
        GameObject card_on_board = Board[slot_index].transform.GetChild(0).GetChild(0).gameObject;
        if (card_on_board == null)
        {
            Debug.Log("Card not found on board");
        }
        textureManager.Texture_Card(card_on_board, id);
        set_card_timer(3, card_on_board);
        Board[slot_index].GetComponent<Animator>().SetTrigger("Reveal");

        #if UNITY_EDITOR
        DestroyImmediate(card_to_move);
        #else
        Destroy(card_to_move);
        #endif

    }

    [ContextMenu("Reduce card timer")]
    public void ReduceCardsTimer()
    {
        for (int i = Board.Length - 1; i >= 0; i--) // Iterate backwards to safely remove
        {
            GameObject card = Board[i].transform.GetChild(0).GetChild(0).gameObject;
            if (card == null)
                continue;

            TMP_Text timerText = card.transform.Find("TimerCanvas/TimerText").GetComponent<TMP_Text>();
            int timer = int.Parse(timerText.text);
            timer--;

            if(isPlayer && i == 1)
                Debug.Log($"Reducing timer for card {card.name} to {timer}");

            if (timer > 0)
                timerText.text = timer.ToString();
            else
            {
                timerText.gameObject.SetActive(false);
                Board[i].GetComponent<Animator>().SetTrigger("Close");
                textureManager.untexture_card(card);
            }
        }
    }

    public void Discard(string id)
    {
        GameObject card_to_discard = null;
        foreach (GameObject card in hand_cards)
            if (card != null && card.name.Substring(0, 3) == id)
                card_to_discard = card;

        hand_cards.Remove(card_to_discard);
        //delete card
        #if UNITY_EDITOR
        DestroyImmediate(card_to_discard);
        #else
        Destroy(card_to_discard);
        #endif
        AddaptAllCardsPositions();
    }

    public void ThrowFirstCard()
    {
        GameObject card_to_discard = hand_cards[0];
        hand_cards.Remove(card_to_discard);
        //delete card
        #if UNITY_EDITOR
        DestroyImmediate(card_to_discard);
        #else
        Destroy(card_to_discard);
        #endif
        AddaptAllCardsPositions();
    }

    public void set_card_timer(int value, GameObject card)
    {
        TMP_Text timerText = card.transform.Find("TimerCanvas/TimerText").GetComponent<TMP_Text>();
        timerText.gameObject.SetActive(true);
        timerText.text = value.ToString();
    }

    public void MoveToTrash(string id)
    {
        Vector3 thickness = new Vector3(0, 0, 0.02f) * trash_spawn_point.lossyScale.z; // Offset for stacking in local space

        GameObject card = Instantiate(card_prefab, trash_spawn_point.position, trash_spawn_point.rotation);
        card.transform.SetParent(gameObject.transform);
        Vector3 offset = (thickness * trash_cards.Count);
        card.transform.position = trash_spawn_point.position + offset;
        card.transform.localScale = card.transform.lossyScale * 3;
        trash_cards.Add(card);
        textureManager.Texture_Card(card, id);
    }

    [ContextMenu("Trash test")]
    public void Trashtest()
    {
        MoveToTrash("001");
    }
}
