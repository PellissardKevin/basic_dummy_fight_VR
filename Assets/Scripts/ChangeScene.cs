using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("before");
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuRoomVR_Kevin", LoadSceneMode.Additive);
        Debug.Log("after");
    }

    public void SwitchToGame()
    {
        SceneManager.UnloadSceneAsync("MenuRoomVR_Kevin");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Arena", LoadSceneMode.Additive);
    }


}
