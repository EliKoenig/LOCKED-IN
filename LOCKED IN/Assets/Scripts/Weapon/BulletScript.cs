using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }
}
