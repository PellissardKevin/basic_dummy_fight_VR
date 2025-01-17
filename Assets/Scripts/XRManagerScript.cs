using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

public class XRManagerScript : MonoBehaviour
{
    public AddScene2Script AddSceneScript;
    public GameObject Player;

    void Start()
    {
        // Check if XR is initialized and active
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            Debug.Log("XR is initialized and active.");
            if (XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                Debug.Log("VR headset is present.");
                AddSceneScript.enabled = true;
            }
            else
            {
                Player.SetActive(true);
                Debug.Log("No VR headset detected.");
            }
        }
        else
        {
            Player.SetActive(true);
            Debug.Log("XR is not initialized.");
        }
    }
}
