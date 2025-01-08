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
        SceneManager.UnloadSceneAsync("MenuRoomVR_Kevin");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Arena", LoadSceneMode.Additive);
    }

    public void BackToMenu()
    {
        SceneManager.UnloadSceneAsync("Arena");
        SceneManager.UnloadSceneAsync("Arena2");
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuRoomVR_Kevin", LoadSceneMode.Additive);
    }


}
