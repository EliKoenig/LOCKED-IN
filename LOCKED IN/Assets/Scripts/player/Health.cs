using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Health : MonoBehaviour
{
    public int health;
    public TextMeshProUGUI healthUI;
    public TextMeshProUGUI shieldUI;
    public UnityEngine.UI.Image shieldImage;
    public int maxHealth;
    public int shield;
    public GameObject shieldParent;
    public GameObject deathText;
    public AudioSource source;
    public AudioClip hurt;


    void Awake()
    {
        healthUI = GameObject.Find("Health Text").GetComponent<TextMeshProUGUI>();
        shieldUI = GameObject.Find("Shield Text").GetComponent<TextMeshProUGUI>();
        shieldParent = GameObject.Find("Shield Parent");
    }

    void Start()
    {
        health = 100;
        maxHealth = 100;
        shield = 0;
    }

    void Update()
    {
        healthUI.SetText(health + " / " + maxHealth);
        if (shield > 0)
        {
            shieldParent.SetActive(true);
            shieldUI.SetText(shield + "");
        }
        else
        {
            shieldParent.SetActive(false);
            shield = 0;
        }
    }

    public void TakeDamage(int damage)
    {
        source.PlayOneShot(hurt);
        if (shield > 0)
        {
            int remainingDamage = Mathf.Max(damage - shield, 0);
            shield = Mathf.Max(shield - damage, 0);
            health -= remainingDamage;
        }
        else
        {
            health -= damage;
        }

        health = Mathf.Max(health, 0);  // Ensures health doesn't go below 0
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");

        deathText.SetActive(true);

        // Pause the game
        Time.timeScale = 0;

        // Display a death message or UI (optional)
        // You can add a UI element to indicate that the game is paused because the player has died
        
    }

    private void ShowDeathScreen()
    {
        // Example implementation
        GameObject deathScreen = GameObject.Find("Death Text Parent"); // Ensure you have a GameObject named "Death Screen" in your scene
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
        }
    }
}
