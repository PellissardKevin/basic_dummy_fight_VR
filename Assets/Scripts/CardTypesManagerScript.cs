using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class CardTypesManagerScript : MonoBehaviour
{
    private string JsonPathType1 => Path.Combine(Application.streamingAssetsPath, "Card_Data/id_to_type1.json");
    private string JsonPathType2 => Path.Combine(Application.streamingAssetsPath, "Card_Data/id_to_type2.json");

    private Dictionary<string, string> idToType1Mapping = new Dictionary<string, string>();
    private Dictionary<string, string> idToType2Mapping = new Dictionary<string, string>();

    void Awake()
    {
        LoadNameMappingFromJson(idToType1Mapping, JsonPathType1);
        LoadNameMappingFromJson(idToType2Mapping, JsonPathType2);
    }

    private void LoadNameMappingFromJson(Dictionary<string, string> Dict_To_Load, string JsonPath)
    {
        if (File.Exists(JsonPath))
        {
            string json = File.ReadAllText(JsonPath);
            var loadedDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Dict_To_Load.Clear();
            foreach (var kvp in loadedDict)
                Dict_To_Load[kvp.Key] = kvp.Value; // Copy data into the passed dictionary
        }
        else
        {
            Debug.Log("No existing JSON found, starting fresh.");
            Dict_To_Load = new Dictionary<string, string>();
        }
    }

    public string GetType1FromID(string id)
    {
        int number = int.Parse(id);
        id = $"{number:D3}"; // Format as a 3-digit number

        if (idToType1Mapping.ContainsKey(id))
            return idToType1Mapping[id];
        else
        {
            Debug.LogWarning($"No type1 found for ID {id}");
            return null;
        }
    }

    public string GetType2FromID(string id)
    {
        int number = int.Parse(id);
        id = $"{number:D3}"; // Format as a 3-digit number

        if (idToType2Mapping.ContainsKey(id))
            return idToType2Mapping[id];
        else
        {
            Debug.LogWarning($"No type2 found for ID {id}");
            return null;
        }
    }

}
