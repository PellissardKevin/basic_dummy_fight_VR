using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    void Awake()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuRoomVR_Kevin", LoadSceneMode.Additive);
    }

    public void SwitchToGame()
    {
        if (SceneManager.GetSceneByName("MenuRoomVR_Kevin").isLoaded)
            SceneManager.UnloadSceneAsync("MenuRoomVR_Kevin");

        if (!SceneManager.GetSceneByName("Arena").isLoaded)
            SceneManager.LoadScene("Arena", LoadSceneMode.Additive);
    }

    public void BackToMenu()
    {
        if (SceneManager.GetSceneByName("Arena").isLoaded)
            SceneManager.UnloadSceneAsync("Arena");

        if (SceneManager.GetSceneByName("Arena2").isLoaded)
            SceneManager.UnloadSceneAsync("Arena2");

        if (!SceneManager.GetSceneByName("MenuRoomVR_Kevin").isLoaded)
            SceneManager.LoadScene("MenuRoomVR_Kevin", LoadSceneMode.Additive);
    }


}
