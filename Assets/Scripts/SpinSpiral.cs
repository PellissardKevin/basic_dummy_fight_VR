using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinSpiral : MonoBehaviour
{
    public bool direction = true;
    public float speed = 1.0f;

    void Update()
    {
        
        transform.Rotate(0, 0, (direction ? 1f : -1f) * speed * Time.deltaTime);
    }
}
