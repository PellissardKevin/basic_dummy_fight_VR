using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerScript : MonoBehaviour
{
    public GameObject PlayerObject;
    public Vector3 PositionMenuPlay;
    public Vector3 OrientationMenuPlay;
    public Vector3 PositionMenuLogin;
    public Vector3 OrientationMenuLogin;

    void Start()
    {
        SendToLogin();
    }

    public void SendToMenu()
    {
        PlayerObject.transform.position = PositionMenuPlay;
        PlayerObject.transform.rotation = Quaternion.Euler(OrientationMenuPlay);
    }
    public void SendToLogin()
    {
        PlayerObject.transform.position = PositionMenuLogin;
        PlayerObject.transform.rotation = Quaternion.Euler(OrientationMenuLogin);
    }

}
