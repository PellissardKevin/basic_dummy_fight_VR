using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRDrawRay : MonoBehaviour
{
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        draw_ray();
    }

    void draw_ray()
    {
        Vector3 Origin = gameObject.transform.position;
        Vector3 Direction = gameObject.transform.forward;

        if (Physics.Raycast(Origin, Direction, out RaycastHit hit, 10))
        {
            // Update the Line Renderer to show the ray
            lineRenderer.SetPosition(0, Origin);       // Start of the ray
            lineRenderer.SetPosition(1, hit.point);       // End at the hit point

        }
        else
        {
            // If no hit, draw the ray to its maximum length
            lineRenderer.SetPosition(0, Origin);
            lineRenderer.SetPosition(1, Origin + Direction * 10);
        }
    }
}
