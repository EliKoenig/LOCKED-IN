using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed;
    public float baseSpeed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.4f;
    public LayerMask groundMask;
    public Transform crouchCheck;

    public float fallMultiplier = 1.75f; // Multiplier for falling gravity

    // Crouch variables
    private Vector3 playerScale = new Vector3(1f, 1f, 1f);
    private Vector3 crouchScale = new Vector3(1f, 0.6f, 1f);
    public float crouchSpeed = 4f;
    public float scaleSpeed = 5f;
    public float sprintSpeed = 8f;

    private float targetSpeed;         // The speed the player is transitioning to
    private float speedLerpTime = 0f;  // Keeps track of time for lerping speed
    private float transitionDuration = 2f;  // Duration to transition speed

    Vector3 velocity;
    bool isGrounded;
    bool isCrouching;
    bool isSprinting;

    void Start()
    {
        // Initialize speeds
        speed = baseSpeed;
        targetSpeed = baseSpeed;
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

        // Crouch Mechanism
        if (Input.GetKey(KeyCode.LeftControl)) // Use your preferred crouch key
        {
            isCrouching = true;
            isSprinting = false; // Prevent sprinting while crouching
            // Smoothly lerp to crouch scale
            transform.localScale = Vector3.Lerp(transform.localScale, crouchScale, Time.deltaTime * scaleSpeed);
            if (isGrounded)
            {
                targetSpeed = crouchSpeed; // Target crouch speed when crouching
                speedLerpTime = Mathf.Min(speedLerpTime + Time.deltaTime / transitionDuration, 1f);  // Increase lerp time up to 1
            }
        }
        else
        {
            
           // if (Physics.Raycast(crouchCheck, Vector3.up, out RaycastHit hit, 0.2f)) 
            //{
                isCrouching = false;
                // Smoothly lerp to stand scale
                transform.localScale = Vector3.Lerp(transform.localScale, playerScale, Time.deltaTime * scaleSpeed * 1.5f);
                if (isGrounded)
                {
                    targetSpeed = baseSpeed; // Target base speed when standing
                    speedLerpTime = Mathf.Min(speedLerpTime + Time.deltaTime / transitionDuration, 1f);  // Increase lerp time up to 1
                }
           // }
        }

        // Sprinting Mechanism
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching && isGrounded) // Use your preferred sprint key
        {
            isSprinting = true;
            targetSpeed = sprintSpeed; // Target sprint speed when sprinting
            speedLerpTime = Mathf.Min(speedLerpTime + Time.deltaTime / transitionDuration, 1f);  // Increase lerp time up to 1
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && !isCrouching) // Ensure we're not crouching when stopping sprint
        {
            isSprinting = false;
            targetSpeed = baseSpeed; // Return to base speed when not sprinting
            if(!isGrounded)
                speedLerpTime = Mathf.Min(speedLerpTime + Time.deltaTime / transitionDuration, 1f);  // Increase lerp time up to 1
        }

        // Smoothly interpolate the speed
        speed = Mathf.Lerp(speed, targetSpeed, speedLerpTime);

        // Reset lerp time once target speed is reached
        if (Mathf.Abs(speed - targetSpeed) < 0.01f)
        {
            speedLerpTime = 0f;
        }

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        if (velocity.y < 2f)
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
