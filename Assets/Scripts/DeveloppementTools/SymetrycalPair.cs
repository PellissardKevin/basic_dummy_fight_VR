using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SymetrycalPair : MonoBehaviour
{
    [Tooltip("The object that should move symmetrically to this one.")]
    public Transform pairedObject;

    [Tooltip("The center point for symmetry, default is the world origin (0, 0, 0).")]
    public Vector3 symmetryCenter = Vector3.zero;

    public bool ActiveRotation = true;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            UpdateSymmetry(); // Ensure symmetry updates when values change in the inspector
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            UpdateSymmetry(); // Update symmetry when dragging in the Scene view
        }
    }

    private void UpdateSymmetry()
    {
        if (pairedObject != null)
        {
            // Calculate the mirrored position for X and Z axes, but keep Y axis the same
            Vector3 mirroredPosition = new Vector3(
                symmetryCenter.x - (transform.position.x - symmetryCenter.x),
                transform.position.y, // Keep the Y axis unchanged
                symmetryCenter.z - (transform.position.z - symmetryCenter.z)
            );

            Vector3 originalRotation = transform.rotation.eulerAngles;
            float mirroredY = (originalRotation.y + 180f) % 360f;
            // Ensure the angle is in the range [0, 360)
            if (mirroredY < 0)
                mirroredY += 360f;


            Quaternion mirroredRotation; // Calculate mirrored rotation

            mirroredRotation = Quaternion.Euler(
                originalRotation.x,
                mirroredY,
                originalRotation.z
            );

            // Copy the scale
            Vector3 mirroredScale = transform.localScale;

            // Update paired object
            pairedObject.position = mirroredPosition;
            if (ActiveRotation)
                pairedObject.rotation = mirroredRotation;
            pairedObject.localScale = mirroredScale;
        }
    }
#endif
}
