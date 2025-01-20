using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrusaderAnimationChoice : MonoBehaviour
{
    private Animator animator; // Reference to the Animator component
    private float interval = 0.9f; // Time in seconds between animation changes

    void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component
        StartCoroutine(ChangeIdleAnimation());
    }

    private IEnumerator ChangeIdleAnimation()
    {
        while (true) // Keep running indefinitely
        {
            yield return new WaitForSeconds(interval); // Wait for the interval time

            // Set a random value for the IdleIndex parameter
            int randomIdle = Random.Range(1, 4); // Random integer between 1 and 3
            animator.SetInteger("IdleIndex", randomIdle);
        }
    }
}
