using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AutoLoginScript : MonoBehaviour
{

    [SerializeField] private TMP_InputField NameField;
    [SerializeField] private TMP_InputField PasswordField;

    public AccessToSocketScript socketScript;

    void Start()
    {
        // Start the coroutine to wait for connection to server
        StartCoroutine(WaitForLogin());
    }

    IEnumerator WaitForLogin()
    {
        // Wait until the variable becomes true
        yield return new WaitUntil(() => socketScript.SocketScript.connected);

        Debug.Log("Ready to log in.");

        // Call the appropriate login function based on the environment
        #if UNITY_EDITOR
        EditorLogin();
        #else
        BuildLogin();
        #endif
    }

    void EditorLogin()
    {
        NameField.text = "Erwan";
        PasswordField.text = "test";
        Debug.Log($"Auto login {NameField.text} {PasswordField.text}");
        PerformLogin();
    }

    void BuildLogin()
    {
        NameField.text = "New";
        PasswordField.text = "test";
        Debug.Log($"Auto login {NameField.text} {PasswordField.text}");
        PerformLogin();
    }

    void PerformLogin()
    {
        socketScript.Login();
    }
}
