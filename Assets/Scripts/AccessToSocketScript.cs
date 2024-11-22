using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AccessToSocketScript : MonoBehaviour
{
    private SocketClient SocketScript;

    public Text statusText; // UI element to display state
    public Text messageText; // UI element to display messages
    public Button startQueueButton; // Button for starting queue
    public Button yesButton; // Button for "Yes" response
    public Button noButton; // Button for "No" response
    public Button endButton; // Button for ending match

    void Start()
    {
        Debug.Log(IsSceneLoaded("Networking"));
        SocketScript = FindObjectOfType<SocketClient>();
        SocketScript.SelfRegister(this);
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

        // Show relevant UI elements based on state
        if (state == "connected")
        {
            Debug.Log("activating button start");
            startQueueButton.gameObject.SetActive(true);
        }
        if (state == "in_match")
            endButton.gameObject.SetActive(true);
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
    }
    public void RespondToMatch(bool response)
    {
        SocketScript.RespondToMatch(response);
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
}
