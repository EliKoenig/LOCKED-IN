using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.4f;
    public LayerMask groundMask;

    public float fallMultiplier = 2.5f; // Multiplier for falling gravity


    // Crouch variables
    private Vector3 playerScale = new Vector3(1f, 1f, 1f);
    private Vector3 crouchScale = new Vector3(1f, 0.6f, 1f);
    public float crouchSpeed = 6f;
    public float scaleSpeed = 5f;

    Vector3 velocity;
    bool isGrounded;
    bool isCrouching => playerScale.y - crouchScale.y > .1f;

    void Start()
    {
        // Store the original height of the character controller
      
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (Input.GetKey(KeyCode.LeftControl)) // Use your preferred crouch key
        {
            // Smoothly lerp to crouch scale
            transform.localScale = Vector3.Lerp(transform.localScale, crouchScale, Time.deltaTime * scaleSpeed);
            speed = crouchSpeed; // Reduce speed while crouching


        }
        else
        {
            // Smoothly lerp to stand scale
            transform.localScale = Vector3.Lerp(transform.localScale, playerScale, Time.deltaTime * scaleSpeed * 1.5f);
            speed = 12f; // Reset speed to original

        }

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        if (velocity.y < 0)

        {
            velocity.y += gravity * fallMultiplier * Time.deltaTime; // Apply increased gravity when falling
        }
        else
        {
            velocity.y += gravity * Time.deltaTime; // Normal gravity when rising
        }

        controller.Move(velocity * Time.deltaTime);



    }
}
