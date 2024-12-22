using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float bulletLifetime = 5f;  // Time in seconds before the bullet self-destructs automatically
    public int bulletDamage = 20;     // Damage dealt by the bullet

    void Start()
    {
        // Destroy the bullet after a certain time to prevent it from lingering in the scene
        Destroy(gameObject, bulletLifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has the tag "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Access the enemy's health component
            MeleeEnemy enemy = collision.gameObject.GetComponent<MeleeEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(bulletDamage);
            }
        }

        // Destroy the bullet itself upon collision with any object
        Destroy(gameObject);
    }
}