using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;

public class CardManagerScript : MonoBehaviour
{
    public SocketClient SocketScript;
    public GameObject card_prefab;
    public string card_id;

    private string baseUrl;
    private string saveFolderPath = "Assets/StreamingAssets/Card_Data";
    private string JsonPathName = "Assets/StreamingAssets/Card_Data/id_to_name.json";
    private string JsonPathType1 = "Assets/StreamingAssets/Card_Data/id_to_type1.json";
    private string JsonPathType2 = "Assets/StreamingAssets/Card_Data/id_to_type2.json";

    private Dictionary<string, string> idToNameMapping = new Dictionary<string, string>();
    private Dictionary<string, string> idToType1Mapping = new Dictionary<string, string>();
    private Dictionary<string, string> idToType2Mapping = new Dictionary<string, string>();

    void Start()
    {
        baseUrl = SocketScript.serverUrl;
    }

    [ContextMenu("Fetch cards into Card Data")]
    void Fetch_Cards()
    {
        baseUrl = SocketScript.serverUrl;
        StartCoroutine(Fetch_Card_Id_Name_Url());
    }

    IEnumerator Fetch_Card_Id_Name_Url()
    {
        UnityWebRequest request = UnityWebRequest.Get(baseUrl + "get_cards_routes");
        Debug.Log($"Requesting {baseUrl + "get_cards_routes"}");
        yield return request.SendWebRequest(); // Send the request and wait for the response

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) // Check if there was an error
        {
            Debug.LogError("Error: " + request.error);
            yield break;
        }

        string jsonResponse = request.downloadHandler.text; //parse the JSON response
        var jsonArray = JsonConvert.DeserializeObject<string[][]>(jsonResponse);

        idToNameMapping.Clear();

        foreach (var item in jsonArray)
        {
            string id = item[0];
            string name = item[1];
            string type1 = item[2];
            string type2 = item[3];
            string imageUrl = baseUrl + "static/" + item[4];

            yield return StartCoroutine(Fetch_Image_and_Store(id, name, imageUrl, type1, type2)); // Fetch the image from the URL
        }
        SaveNameMappingToJson(idToNameMapping, JsonPathName);
        SaveNameMappingToJson(idToType1Mapping, JsonPathType1);
        SaveNameMappingToJson(idToType2Mapping, JsonPathType2);
    }

    IEnumerator Fetch_Image_and_Store(string id, string name, string imageUrl, string type1, string type2)
    {
        Debug.Log($"Requesting {imageUrl}");
        UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return imageRequest.SendWebRequest();

        if (imageRequest.result == UnityWebRequest.Result.ConnectionError || imageRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error fetching image: " + imageRequest.error + " " + imageUrl);
            yield break; // Stop the coroutine if there is an error
        }
        Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest); // Get the texture from the response

        string formattedId = int.Parse(id).ToString("D3");
        string fileName = $"{formattedId}.png";
        string filePath = Path.Combine(saveFolderPath, fileName);

        byte[] imageBytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, imageBytes); // Save the PNG to the specified path
        idToNameMapping[formattedId] = name;
        idToType1Mapping[formattedId] = type1;
        idToType2Mapping[formattedId] = type2;
    }


    [ContextMenu("Clear Folder")]
    public void ClearFolder()
    {
        if (Directory.Exists(saveFolderPath))
        {
            Directory.Delete(saveFolderPath, true); // Delete the folder and its contents
            Debug.Log($"Cleared folder: {saveFolderPath}");
        }
        Directory.CreateDirectory(saveFolderPath); // Ensure folder exists
    }

    public void Create_Card_From_ID(string id, Vector3 position = default)
    {
        if (position == default)
            position = Vector3.zero;

        // Format the ID with leading zeros
        string formattedId = int.Parse(id).ToString("D3");
        string[] files = Directory.GetFiles(saveFolderPath, $"{formattedId}_*.png");

        if (files.Length == 0)
        {
            Debug.LogError($"No image found for ID: {id} in folder: {saveFolderPath}");
            return;
        }

        string filePath = files[0]; // Assume the first match is the desired file
        string card_name = Path.GetFileNameWithoutExtension(filePath);

        // Load the image as a Texture2D
        byte[] imageBytes = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2); // Initial size doesn't matter; it will be replaced
        if (!texture.LoadImage(imageBytes))
        {
            Debug.LogError($"Failed to load image from file: {filePath}");
            return;
        }

        // Instantiate the prefab
        GameObject newObject = Instantiate(card_prefab, transform);
        newObject.name = card_name;

        newObject.transform.localPosition = position;

        // Find the child named "Face" and apply the texture to its material
        Transform faceTransform = newObject.transform.Find("Face");
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

        faceRenderer.material.mainTexture = texture;
        Debug.Log($"Successfully instantiated prefab and applied image for ID: {id}");
    }

    [ContextMenu("Instanciate Card")]
    public void CreateCard()
    {
        Create_Card_From_ID(card_id);
    }


    [ContextMenu("Create all cards Card")]
    public void InstantiateAllCardsFromFolder()
    {
        // Ensure the folder exists
        if (!Directory.Exists(saveFolderPath))
        {
            Debug.LogError($"Folder does not exist: {saveFolderPath}");
            return;
        }

        // Get all PNG files in the folder
        string[] files = Directory.GetFiles(saveFolderPath, "*.png");

        if (files.Length == 0)
        {
            Debug.Log("No image files found in the folder.");
            return;
        }

        Debug.Log($"Found {files.Length} image files in the folder.");

        foreach (string filePath in files)
        {
            // Extract the filename without extension
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            // Split the filename to get the ID (assuming filenames are in the format 'id_name.png')
            string[] parts = fileName.Split('_');
            if (parts.Length < 2)
            {
                Debug.LogWarning($"Skipping file with invalid name format: {fileName}");
                continue;
            }

            string id = parts[0]; // Extract the ID part

            // Instantiate the prefab with the image
            Create_Card_From_ID(id);
        }
    }

    private void SaveNameMappingToJson(Dictionary<string, string> Dict_To_Save, string JsonPath)
    {
        LoadNameMappingFromJson(Dict_To_Save, JsonPath);

        // Add new key-value pairs to the dictionary
        foreach (var entry in Dict_To_Save)
        {
            if (!Dict_To_Save.ContainsKey(entry.Key))
                Dict_To_Save[entry.Key] = entry.Value;
        }

        // Convert the dictionary to JSON
        string json = JsonConvert.SerializeObject(Dict_To_Save, Formatting.Indented);

        Debug.Log(JsonPath);
        // Write the updated JSON to the file
        File.WriteAllText(JsonPath, json);
        Debug.Log($"Mapping saved to {JsonPath}");
    }

    public void LoadNameMappingFromJson(Dictionary<string, string> Dict_To_Load, string JsonPath)
    {
        if (File.Exists(JsonPath))
        {
            string json = File.ReadAllText(JsonPath);
            Debug.Log("Name mapping loaded from JSON");
            Dict_To_Load = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }
        else
        {
            Debug.Log("No existing JSON found, starting fresh.");
            Dict_To_Load = new Dictionary<string, string>();
        }
    }
}
