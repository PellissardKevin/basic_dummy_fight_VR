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


    private string saveFolderPath = "Assets/StreamingAssets/Card_Data";
    private string JsonPath = "Assets/StreamingAssets/Card_Data/id_to_name.json";

    private Dictionary<string, string> idToNameMapping = new Dictionary<string, string>();

    public Text debugobj;
    public Text debugobj2;

    void Start()
    {
        LoadNameMappingFromJson();
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
            deck_cards.Add(Instantiate(card_prefab, deck_spawn_point.position + previous, deck_spawn_point.rotation));
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
            Texture_Card(picked_card, id);
        }
        catch (System.Exception ex) // Catching a general exception
        {
            debugobj.text = $"Error: {ex.Message}";
        }
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

    private (Texture2D, string) Find_Texture(string id)
    {
        // Format the ID with leading zeros
        string formattedId = int.Parse(id).ToString("D3");
        string folderPath = Path.Combine(Application.streamingAssetsPath, "Card_Data");
        debugobj2.text = $"path: {folderPath}";
        string[] files = Directory.GetFiles(folderPath, $"{formattedId}.png");

        if (files.Length == 0)
        {
            Debug.LogError($"No image found for ID: {formattedId} in folder: {saveFolderPath}");
            return (null, null);
        }

        string filePath = files[0]; // Assume the first match is the desired file
        string card_name = $"{id}_{idToNameMapping[formattedId]}";

        // Load the image as a Texture2D
        byte[] imageBytes = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2); // Initial size doesn't matter; it will be replaced
        if (!texture.LoadImage(imageBytes))
        {
            Debug.LogError($"Failed to load image from file: {filePath}");
            return (null, null);
        }
        return (texture, card_name);
    }

    public void Texture_Card(GameObject card, string id)
    {
        (Texture2D texture, string card_name) = Find_Texture(id);
        if (texture == null || card_name == null)
            return;
        card.name = card_name;
        // Find the child named "Face" and apply the texture to its material
        Transform faceTransform = card.transform.Find("Face");
        if (faceTransform == null)
        {
            Debug.LogError("Child named 'Face' not found in the instantiated prefab.");
            return;
        }

        Renderer faceRenderer = faceTransform.GetComponent<Renderer>();
        if (faceRenderer == null)
        {
            Debug.LogError("Renderer component not found on the 'Face' child object.");
            return;
        }
        Material newMaterial = new Material(Shader.Find("Standard"));
        newMaterial.mainTexture = texture;
        faceRenderer.material = newMaterial;
        Debug.Log($"Successfully instantiated prefab and applied image for ID: {id}");
    }

    public void Pick_Cards(string jsonString)
    {
        jsonString = Regex.Replace(jsonString, @"[^0-9,]", "");
        string[] card_ids = jsonString.Split(new string[] { "," }, StringSplitOptions.None);
        foreach (string id in card_ids)
            MoveCardToHand(id);
    }

    public void LoadNameMappingFromJson()
    {
        string folderPath = Path.Combine(Application.streamingAssetsPath, "Card_Data/id_to_name.json");
        if (File.Exists(folderPath))
        {
            string json = File.ReadAllText(folderPath);
            idToNameMapping = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Debug.Log("Name mapping loaded from JSON");
        }
        else
            Debug.Log("No existing JSON found, starting fresh.");
    }
}

