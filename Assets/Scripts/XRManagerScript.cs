using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

public class XRManagerScript : MonoBehaviour
{
    public GameObject Player;
    public GameObject AddScene2Object;

    void Start()
    {
        // Check if XR is initialized and active
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            Debug.Log("XR is initialized and active.");
            if (XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                Debug.Log("VR headset is present.");
                AddScene2Object.SetActive(true);
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
