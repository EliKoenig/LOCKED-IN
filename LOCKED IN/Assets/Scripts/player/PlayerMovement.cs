using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public GameObject playerMesh;

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

    // Ladder variables
    private bool isClimbing = false;
    public float climbSpeed = 3f;
    private bool isJumpingOffLadder = false;
    private bool nearLadder = false; // Tracks if player is near the ladder

    void Start()
    {
        // Initialize speeds
        speed = baseSpeed;
        targetSpeed = baseSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(isClimbing);
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        if (isGrounded && velocity.y < 0 && !isClimbing)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        if (isClimbing)
        {
            // Ladder climbing movement
            float climbDirection = Input.GetAxis("Vertical");
            move = transform.up * climbDirection * climbSpeed;

            // Disable gravity while climbing
            velocity.y = 0f;

            // Exit climbing if grounded and moving away from the ladder
           /* if (isGrounded && (x != 0 || z != 0))
            {
                isClimbing = false;
            }*/

            // Allow jumping while on the ladder
            if (Input.GetButtonDown("Jump"))
            {
                isClimbing = false;
                isJumpingOffLadder = true;
                velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            // Allow player to start climbing if near the ladder and pressing "Vertical"
            if (nearLadder)
            {
                isClimbing = true;
            }
            else
            {
                // Regular movement
                HandleMovement(move);
            }
        }

        controller.Move(move * speed * Time.deltaTime);
        HandleJumpAndGravity();
    }

    private void HandleMovement(Vector3 move)
    {
        // Crouch Mechanism
        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
            isSprinting = false;
            transform.localScale = Vector3.Lerp(transform.localScale, crouchScale, Time.deltaTime * scaleSpeed);
            if (isGrounded)
            {
                targetSpeed = crouchSpeed;
                speedLerpTime = Mathf.Min(speedLerpTime + Time.deltaTime / transitionDuration, 1f);
            }
        }
        else
        {
            isCrouching = false;
            transform.localScale = Vector3.Lerp(transform.localScale, playerScale, Time.deltaTime * scaleSpeed * 1.5f);
            if (isGrounded)
            {
                targetSpeed = baseSpeed;
                speedLerpTime = Mathf.Min(speedLerpTime + Time.deltaTime / transitionDuration, 1f);
            }
        }

        // Sprinting Mechanism
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching && isGrounded)
        {
            isSprinting = true;
            targetSpeed = sprintSpeed;
            speedLerpTime = Mathf.Min(speedLerpTime + Time.deltaTime / transitionDuration, 1f);
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            isSprinting = false;
            targetSpeed = baseSpeed;
            if (!isGrounded)
                speedLerpTime = Mathf.Min(speedLerpTime + Time.deltaTime / transitionDuration, 1f);
        }

        speed = Mathf.Lerp(speed, targetSpeed, speedLerpTime);
        if (Mathf.Abs(speed - targetSpeed) < 0.01f)
        {
            speedLerpTime = 0f;
        }
    }

    private void HandleJumpAndGravity()
    {
        if (!isClimbing)
        {
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            if (velocity.y < 2f)
            {
                velocity.y += gravity * fallMultiplier * Time.deltaTime;
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
            }
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            nearLadder = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            nearLadder = false;
            isClimbing = false;
            isJumpingOffLadder = false;
        }
    }
}
