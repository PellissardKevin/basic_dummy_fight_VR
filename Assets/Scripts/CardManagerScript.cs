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
    private string saveFolderPath = "Card_Data";

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

        foreach (var item in jsonArray)
        {
            string id = item[0];      // Column 0: ID
            string name = item[1];
            string imageUrl = baseUrl + "static/" + item[2]; // Column 1: Image URL

            StartCoroutine(Fetch_Image_and_Store(id, name, imageUrl)); // Fetch the image from the URL
        }
    }

    IEnumerator Fetch_Image_and_Store(string id, string name, string imageUrl)
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
        string fileName = $"{formattedId}_{name}.png";
        string filePath = Path.Combine(saveFolderPath, fileName);

        // Encode the texture to PNG
        byte[] imageBytes = texture.EncodeToPNG();

        // Save the PNG to the specified path
        File.WriteAllBytes(filePath, imageBytes);
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
}
