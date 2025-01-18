using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyScript : MonoBehaviour
{
    public VirtualKeyBoard keyboard;

    public void find_keyboard()
    {
        keyboard = GameObject.FindObjectOfType<VirtualKeyBoard>();
    }

    public void OnClick()
    {
        if (keyboard != null)
        {
            TMP_Text buttonText = GetComponentInChildren<TMP_Text>();
            if (buttonText == null)
                Debug.Log("No text component found!");

            keyboard.OnKeyPress(buttonText.text);
        }
    }
}
