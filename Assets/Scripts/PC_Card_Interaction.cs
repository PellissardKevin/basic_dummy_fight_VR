using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC_Card_Interaction : MonoBehaviour
{
    public List<GameObject> cardList = new List<GameObject>();

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            DetectCardClick();

        }
    }

    void DetectCardClick()
    {
        // Cast a ray from the camera to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object is a child of an object with the tag "Card"
            Transform current = hit.collider.transform;
            while (current != null)
            {
                if (current.CompareTag("Card"))
                {
                    SelectCard(current.gameObject);
                    break;
                }
                current = current.parent;
            }
        }
    }

    void SelectCard(GameObject card)
    {
        if (cardList.Contains(card))
        {
            cardList.Remove(card);
            RemoveGlow(card);
        }
        else
        {
            cardList.Add(card);
            ApplyGlow(card);
        }
    }

    void ApplyGlow(GameObject card)
    {
        Renderer renderer = card.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            // Assuming the card material supports emission
            renderer.material.EnableKeyword("_EMISSION");
            renderer.material.SetColor("_EmissionColor", Color.yellow * 0.3f);
        }
    }

    void RemoveGlow(GameObject card)
    {
        Renderer renderer = card.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material.DisableKeyword("_EMISSION");
        }
    }
}
