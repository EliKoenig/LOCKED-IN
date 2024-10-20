using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderScript : MonoBehaviour
{
    public Transform body;
    public bool inside = false;
    public CharacterController FPSInput;
    public float climbSpeed = 30f;

    void Start()
    {
        FPSInput = GetComponent<CharacterController>();
        inside = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Ladder")
        {
            inside = true; // Set inside to true when entering the ladder
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Ladder")
        {
            inside = false; // Set inside to false when exiting the ladder
        }
    }

    void Update()
    {
        Debug.Log(inside);

        // If the player is inside the ladder area
        if (inside)
        {
            // Override normal gravity and movement for climbing
            FPSInput.Move(Vector3.zero); // Prevents normal movement while on the ladder

            // Move up if 'W' is pressed
            if (Input.GetKey("w"))
            {
                body.transform.position += Vector3.up * climbSpeed * Time.deltaTime;
            }
            // Move down if 'S' is pressed
            if (Input.GetKey("s"))
            {
                body.transform.position += Vector3.down * climbSpeed * Time.deltaTime;
            }
        }
    }
}
