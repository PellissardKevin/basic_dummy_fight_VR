using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class XRPushButtons : MonoBehaviour
{
    private ActionBasedController XRControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        XRControllerScript = gameObject.GetComponent<ActionBasedController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (XRControllerScript.activateAction.action.IsPressed())
        {
            PushButton();
        }
    }

    public void PushButton()
    {
        Vector3 Origin = gameObject.transform.position;
        Vector3 Direction = gameObject.transform.forward;

        if (Physics.Raycast(Origin, Direction, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("UI")))
        {
            Button button = hit.collider.GetComponent<Button>();
            if (button != null)
            {
                // If it's a button, invoke the OnClick event
                button.onClick.Invoke();
                Debug.Log("Button clicked!");
            }
            else
            {
                Debug.Log("Hit object is not a button.");
            }

        }
    }
}
