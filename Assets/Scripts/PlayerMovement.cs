using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 5.0f;
    private float rotationSpeed = 5f;
    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;
    private float jumpForce = 5.0f;

    private CharacterController characterController;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
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

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 move = (cameraForward * moveZ + cameraRight * moveX) * speed;
        animator.SetFloat("Speed", move.magnitude);

        Vector3 newPosition = transform.position + move * Time.deltaTime;
        //Vector3 newPosition = rb.position + move * Time.deltaTime;
        //rb.MovePosition(newPosition);
        characterController.Move(move * Time.deltaTime);

        if (move.magnitude > 0.1f)  // To avoid unwanted rotation when not moving
        {
            //Quaternion targetRotation = Quaternion.LookRotation(move);
            //rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, Time.deltaTime * rotationSpeed);  // rotationSpeed controls the rotation speed
        }

        // Rotate the character based on mouse input
        Quaternion characterRotation = Quaternion.Euler(0, rotationY * rotationSpeed, 0);
        //rb.MoveRotation(rb.rotation * characterRotation);
        transform.rotation *= characterRotation;

        /*if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool("Jump", true);
        }*/
    }

    private void FixedUpdate()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0f; // Raise the origin slightly
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, 0.1f);
        if (isGrounded && animator.GetBool("Jump"))
            animator.SetBool("Jump", false);
        //animator.SetFloat("JumpHeight", rb.velocity.y);
    }
}
