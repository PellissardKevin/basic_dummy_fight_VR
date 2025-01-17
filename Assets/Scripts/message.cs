using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class message : MonoBehaviour
{
    // Start is called before the first frame update
    public ActionBasedController XRscript;
    public Transform LeftController;
    public Transform RightController;
    public LineRenderer lineRenderer;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Debug.Log ("hello world");

        if (XRscript.selectAction.action.WasPressedThisFrame())
        {
            Debug.Log("Hello, select action triggered!");
        }

        // Check if the activate action is triggered
        if (XRscript.activateAction.action.WasPressedThisFrame())
        {
            Debug.Log("Hello, activate action triggered!");
        }
        
        draw_ray(LeftController);
        draw_ray(RightController);
    }

    void draw_ray(Transform controller)
    {
        Vector3 Origin = controller.position;
        Vector3 Direction = controller.forward;

        Debug.DrawRay (Origin, Direction*10, Color.green);
        if (Physics.Raycast(Origin, Direction, out RaycastHit hit, 10))
        {
            // Update the Line Renderer to show the ray
            lineRenderer.SetPosition(0, Origin);       // Start of the ray
            lineRenderer.SetPosition(1, hit.point);       // End at the hit point

            // Log the hit
            Debug.Log($"Hit: {hit.collider.gameObject.name} at {hit.point}");
        }
        else
        {
            // If no hit, draw the ray to its maximum length
            lineRenderer.SetPosition(0, Origin);
            lineRenderer.SetPosition(1, Origin + Direction * 10);
        }
    }
}
