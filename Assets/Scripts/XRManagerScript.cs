using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.SpatialTracking;

public class XRManagerScript : MonoBehaviour
{
    public GameObject AddScene2Object;

    public GameObject XR_Rig;
    public GameObject leftHand;
    public GameObject rightHand;

    public Camera xrCamera;

    void Start()
    {
        // Check if XR is initialized and active
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            Debug.Log("XR is initialized and active.");
            if (XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                Debug.Log("VR headset is present.");
                //AddScene2Object.SetActive(true);
            }
            else
            {
                Debug.Log("No VR headset detected.");
            }
        }
        else
        {
            Debug.Log("XR is not initialized.");
            deactivateXR();
        }
    }

    public void deactivateXR()
    {
        leftHand.SetActive(false);
        rightHand.SetActive(false);

        var allComponents = xrCamera.GetComponents<Component>();
        foreach (var component in allComponents)
        {
            if (component.GetType().Name == "TrackedPoseDriver")
            {
                component.GetType().GetProperty("enabled").SetValue(component, false, null);
            }
        }
    }
}
