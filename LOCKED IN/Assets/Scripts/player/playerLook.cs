using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public float sensitivity = 100f;
    public Transform playerBody;   // The player's body (the object that rotates left and right)
    public GameObject currentGun;
    public GameObject cameraPos;   // The empty GameObject attached to the player that represents the camera's desired position

    float xRotation = 0f;

    void Start()
    {
        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Update the camera position to match the cameraPos GameObject's position
        transform.position = cameraPos.transform.position;

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
        Quaternion playerRotation = playerBody.rotation;
        transform.rotation = Quaternion.Euler(xRotation, playerRotation.eulerAngles.y, 0f);
    }
}