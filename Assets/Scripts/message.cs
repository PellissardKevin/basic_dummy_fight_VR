using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class message : MonoBehaviour
{
    // Start is called before the first frame update
    public ActionBasedController LeftControllerscript;
    public ActionBasedController RightControllerscript;
    public Transform LeftController;
    public Transform RightController;
    public LineRenderer lineRendererLeft;
    public LineRenderer lineRendererRight;
    public PupitreScript pupitreScript;

    private bool isDragging;
    private GameObject draggedObject;
    private Vector3 originalPosition;
    private string drag_phase;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Debug.Log("hello world");

        if (LeftControllerscript.selectAction.action.WasPressedThisFrame())
        {
            Debug.Log("Hello, select action triggered!");
            DetectCardXr(LeftController, LeftControllerscript);
        }

        // Check if the activate action is triggered
        if (LeftControllerscript.activateAction.action.WasPressedThisFrame())
        {
            Debug.Log("Hello, activate action triggered!");
        }

        draw_ray(LeftController, lineRendererLeft);
        draw_ray(RightController, lineRendererRight);

    }


    void draw_ray(Transform controller, LineRenderer Line_Renderer)
    {
        Vector3 Origin = controller.position;
        Vector3 Direction = controller.forward;

        if (Physics.Raycast(Origin, Direction, out RaycastHit hit, 10))
        {
            // Update the Line Renderer to show the ray
            Line_Renderer.SetPosition(0, Origin);       // Start of the ray
            Line_Renderer.SetPosition(1, hit.point);       // End at the hit point

        }
        else
        {
            // If no hit, draw the ray to its maximum length
            Line_Renderer.SetPosition(0, Origin);
            Line_Renderer.SetPosition(1, Origin + Direction * 10);
        }
    }

    void DetectCardXr(Transform controller, ActionBasedController Controllerscript)
    {
        Vector3 Origin = controller.position;
        Vector3 Direction = controller.forward;

        if (Physics.Raycast(Origin, Direction, out RaycastHit hit, 10))
        {
            foreach (GameObject obj in pupitreScript.hand_cards)
            {
                if (hit.collider.gameObject == obj)
                {
                    StartDragging(obj);
                    break;
                }
            }
        }
    }

    private void StartDragging(GameObject obj)
    {
        isDragging = true;
        draggedObject = obj;
        originalPosition = obj.transform.position;
        // drag_phase = gameSocketScript.current_phase;
        // set_effects(draggedObject.name.Substring(0, 3));
        Debug.Log($"Start dragging {obj.name}");
    }
}

