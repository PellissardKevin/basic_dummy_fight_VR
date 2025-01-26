using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 5.0f;
    private float rotationSpeed = 10f;
    private float verticalSpeed = 2f;
    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;
    private float jumpForce = 5.0f;

    private CharacterController characterController;
    public GameObject CameraOffset;

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Get input for movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float rotationY = Input.GetAxis("Mouse X");
        float rotationX = Input.GetAxis("Mouse Y");

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 move = (cameraForward * moveZ + cameraRight * moveX) * speed;
        Vector3 newPosition = transform.position + move * Time.deltaTime;
        characterController.Move(move * Time.deltaTime);


        // Rotate the character based on mouse input
        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            transform.rotation *= Quaternion.Euler(0, rotationY * rotationSpeed, 0);

            CameraOffset.transform.rotation *= Quaternion.Euler(-rotationX * verticalSpeed, 0, 0);
            Vector3 eulerAngles = CameraOffset.transform.rotation.eulerAngles;
            eulerAngles.x = ClampAngle(eulerAngles.x, -45f, 45f);
            CameraOffset.transform.rotation = Quaternion.Euler(eulerAngles);
        }


    }

    private void FixedUpdate()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0f; // Raise the origin slightly
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, 0.1f);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle > 180f) angle -= 360f; // Convert large angles to a range of -180 to 180
        return Mathf.Clamp(angle, min, max);
    }
}
