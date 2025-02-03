using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    //public float sensitivity = 100f;  // The player's body (the object that rotates left and right)
    //public GameObject currentGun;
    /// <summary>
    // The empty GameObject attached to the player that represents the camera's desired position
    /// </summary>

    // float xRotation = 0f;

    /*
    void LateUpdate()
    {
        // Update the camera position to match the cameraPos GameObject's position
        

        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // Adjust the camera's vertical rotation (up and down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limit rotation to avoid looking too far up/down

        // Apply the vertical rotation to the camera only (looking up and down)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate the player horizontally (left-right) based on mouse X movement
        playerBody.Rotate(Vector3.up * mouseX);

        // Now, match the camera's Y rotation with the player's rotation to ensure it looks left and right with the player
        //Quaternion playerRotation = playerBody.rotation;
        //transform.rotation = Quaternion.Euler(xRotation, playerRotation.eulerAngles.y, 0f);
        if(mouseX > 0f && mouseY > 0f) 
        Debug.Log($"MouseX: {mouseX}, MouseY: {mouseY}");
    }
    private void Update()
    {
        

    }
    /*

    */

    public Transform player;         // Player transform
    public Transform cameraTarget;   // Assign "CameraHolder" in Inspector
    private Vector2 rotation;

    private Quaternion originalRotation;

    public float HORIZONTAL_ROTATION_SPEED = 2f;
    public float VERTICAL_ROTATION_SPEED = 2f;
    private const int VERTICAL_ROTATION_MIN = -90;
    private const int VERTICAL_ROTATION_MAX = 90;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        originalRotation = transform.localRotation;
    }

    private void Update()
    {
        // Get mouse input
        rotation.x = Input.GetAxisRaw("Mouse X") * HORIZONTAL_ROTATION_SPEED;
        rotation.y += Input.GetAxisRaw("Mouse Y") * VERTICAL_ROTATION_SPEED;
        rotation.y = Mathf.Clamp(rotation.y, VERTICAL_ROTATION_MIN, VERTICAL_ROTATION_MAX);

        // Rotate the player left/right (yaw)
        player.localRotation *= Quaternion.AngleAxis(rotation.x, Vector3.up);
    }

    private void LateUpdate()
    {
        if (cameraTarget != null)
        {
            // Move camera to CameraHolder's position
            transform.position = cameraTarget.position;

            // Apply vertical rotation (up/down) while keeping horizontal rotation from the player
            transform.rotation = player.rotation * Quaternion.AngleAxis(rotation.y, Vector3.left);
        }
    }
}