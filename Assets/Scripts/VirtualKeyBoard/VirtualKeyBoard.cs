using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VirtualKeyBoard : MonoBehaviour
{
    public GameObject keyPrefab;
    public TMP_InputField SelectedInputField;
    public TMP_InputField passwordInputField;
    public Button ConnexionButton;

    public float A1 = 180f;
    public float R1 = 5f;
    public float A2 = 180f;
    public float R2 = 5f;
    public float A3 = 180f;
    public float R3 = 5f;

    [ContextMenu("Generate Keyboard Arc")]
    public void GenerateKeyboardArc()
    {
        if (keyPrefab == null)
        {
            Debug.LogError("Button Prefab is not assigned.");
            return;
        }

        ClearKeyboard();
        create_arc(R1, A1, new string[] { "A", "Z", "E", "R", "T", "Y", "U", "I", "O", "P", "\u2190", "7", "8", "9" });
        create_arc(R2, A2, new string[] { "Q", "S", "D", "F", "G", "H", "J", "K", "L", "M", "4", "5", "6" });
        create_arc(R3, A3, new string[] { "Shift", "W", "X", "C", "V", "B", "N", "Enter", "1", "2", "3" });

    }

    public void create_arc(float arcRadius, float arcAngle, string[] keys)
    {
        float numberOfButtons = keys.Length;

        // Adjusted angle step to account for extra space
        float adjustedArcAngle = arcAngle;
        int totalWeight = 0;

        // Calculate total weight (1 for normal keys, 2 for special keys, etc.)
        foreach (string key in keys)
        {
            if (key == "Shift" || key == "Enter" || key == "\u2190") // Add more special keys here
                totalWeight += 2; // Special keys take extra space
            else
                totalWeight += 1; // Normal keys take 1 space
        }

        float angleStep = adjustedArcAngle / (totalWeight - 1); // Angle between weighted keys
        float startAngle = -adjustedArcAngle / 2; // Starting angle
        float currentAngle = startAngle;

        for (int i = 0; i < numberOfButtons; i++)
        {
            string keyName = keys[i];

            // Determine weight of the current key
            int keyWeight = 1;
            if (keyName == "Shift" || keyName == "Enter" || keyName == "\u2190")
                keyWeight = 2;

            // Calculate angle span for the key
            float keyAngleSpan = angleStep * keyWeight;

            // Calculate position (middle of the angle span)
            float middleAngle = currentAngle + (keyAngleSpan / 2);
            float angleRad = Mathf.Deg2Rad * middleAngle;

            float x = Mathf.Sin(angleRad) * arcRadius;
            float y = Mathf.Cos(angleRad) * arcRadius;

            // Instantiate the button prefab
            GameObject button = Instantiate(keyPrefab, transform);

            button.transform.localPosition = new Vector3(x, y, 0);
            button.transform.localRotation = Quaternion.Euler(0, 0, -middleAngle);
            if (keyWeight == 2)
            {
                RectTransform rectTransform = button.GetComponent<RectTransform>();
                 Vector2 size = rectTransform.sizeDelta;
                size.x = 35;
                rectTransform.sizeDelta = size;
            }

            TMPro.TextMeshProUGUI buttonText = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            buttonText.text = keyName;
            button.name = $"Key_{keyName}";

            KeyScript keyScript = button.GetComponent<KeyScript>();
            keyScript.keyboard = this;

            currentAngle += keyAngleSpan;
        }
    }

    [ContextMenu("Clear Keyboard")]
    public void ClearKeyboard()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
    }

    public void OnKeyPress(string key)
    {
        if (key == "\u2190")
            OnBackspace();
        else if (key == "Shift")
            Shift();
        else if (key == "Enter")
            Enter();
        else
            SelectedInputField.text += key;
    }

    public void Shift()
    {
        foreach (Transform child in transform)
        {
            Button button = child.GetComponent<Button>();

            // Ensure the button is valid and has a text component
            if (button != null && button.GetComponentInChildren<TMP_Text>() != null)
            {
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>(); // Get the actual text component
                string text = buttonText.text;

                if (text.Length == 1 && char.IsLetter(text[0])) // Check if the text is a single letter
                {
                    if (char.IsUpper(text[0]))
                        buttonText.text = text.ToLower();
                    else
                        buttonText.text = text.ToUpper();
                }
            }
        }
    }

    public void Enter()
    {
        if (SelectedInputField == passwordInputField)
           ConnexionButton.onClick.Invoke();
        else
            SelectedInputField = passwordInputField;
    }

    public void OnBackspace()
    {
        if (SelectedInputField == null)
            return;
        if (SelectedInputField.text.Length > 0)
            SelectedInputField.text = SelectedInputField.text.Substring(0, SelectedInputField.text.Length - 1);
    }

    public void SelectInputField(TMP_InputField inputField)
    {
        SelectedInputField = inputField;
    }

}
