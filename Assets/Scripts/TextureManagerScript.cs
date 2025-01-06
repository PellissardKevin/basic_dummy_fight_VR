using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class TextureManagerScript : MonoBehaviour
{

    private string saveFolderPath = "Assets/StreamingAssets/Card_Data";
    private string JsonPath = "Assets/StreamingAssets/Card_Data/id_to_name.json";

    private Dictionary<string, string> idToNameMapping = new Dictionary<string, string>();

    void Awake()
    {
        LoadNameMappingFromJson();
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

    private (Texture2D, string) Find_Texture(string id)
    {
        // Format the ID with leading zeros
        string formattedId = int.Parse(id).ToString("D3");
        string folderPath = Path.Combine(Application.streamingAssetsPath, "Card_Data");
        string[] files = Directory.GetFiles(folderPath, $"{formattedId}.png");

        if (files.Length == 0)
        {
            Debug.LogError($"No image found for ID: {formattedId} in folder: {saveFolderPath}");
            return (null, null);
        }

        string filePath = files[0]; // Assume the first match is the desired file
        string card_name = $"{formattedId}_{idToNameMapping[formattedId]}";

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
}
