using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class AccessToSocketScript : MonoBehaviour
{
    public SocketClient SocketScript;
    public PlayerManagerScript playerManagerScript;

    public Text statusText; // UI element to display state
    public Text messageText; // UI element to display messages
    public Button startQueueButton; // Button for starting queue
    public Button yesButton; // Button for "Yes" response
    public Button noButton; // Button for "No" response
    public Button endButton; // Button for ending match
    public Text connexion;

    public GameObject Spiral1;
    public GameObject Spiral2;

    [SerializeField] private TMP_InputField NameField;
    [SerializeField] private TMP_InputField PasswordField;

    void Start()
    {
        //Debug.Log(IsSceneLoaded("Networking"));
        SocketScript = FindObjectOfType<SocketClient>();
        SocketScript.SelfRegister(this);
    }
    void Update()
    {
        if (SocketScript.connected && connexion.gameObject.activeSelf)
            connexion.gameObject.SetActive(false);
        if (!SocketScript.connected && !connexion.gameObject.activeSelf)
            connexion.gameObject.SetActive(true);
    }

    public void CallStartQueue()
    {
        SocketScript.StartQueue();
    }

    public void UpdateState(string state)
    {
        statusText.text = "State: " + state;
        UpdateUI(state);
    }

    public void UpdateUI(string state)
    {
        // Reset all UI elements
        messageText.gameObject.SetActive(false);
        startQueueButton.gameObject.SetActive(false);
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        endButton.gameObject.SetActive(false);

        Spiral1.SetActive(false);
        Spiral2.SetActive(false);

        // Show relevant UI elements based on state
        if (state == "verified")
            startQueueButton.gameObject.SetActive(true);
        if (state == "in_match")
            endButton.gameObject.SetActive(true);
        if (state == "waiting")
        {
            Spiral1.SetActive(true);
            Spiral2.SetActive(true);
        }
    }

    public void server_recieved_match_response()
    {
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }
    public void prompt_match(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);

        Spiral1.SetActive(false);
        Spiral2.SetActive(false);
    }
    public void RespondToMatch(bool response)
    {
        SocketScript.RespondToMatch(response);
    }

    public void Login()
    {
        string username = NameField.text;
        string password = PasswordField.text;
        SocketScript.LogIn(username, password);
    }

    private bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName && scene.isLoaded)
            {
                return true;
            }
        }
        return false;
    }
    public void EndMatch()
    {
        SocketScript.EndMatch();
    }
    public void Wrong_Login()
    {
        Debug.Log("Wrong login");
    }
    public void Good_Login()
    {
        playerManagerScript.SendToMenu();
        Debug.Log("Good login");
    }

}
