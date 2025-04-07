using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;

public class Health : MonoBehaviour
{
    public ProjectileScript projectileScript;
    public GPProjectileScript gpProjectileScript;
    public AudioProjectileScript audioProjectileScript;
    public int health;
    public TextMeshProUGUI healthUI;
    public TextMeshProUGUI shieldUI, timeUI, scoreUI, timerUI, deathUI;
    public UnityEngine.UI.Image shieldImage;
    public int maxHealth;
    public int shield;
    public GameObject shieldParent, crosshair;
    public GameObject deathText;
    public AudioSource source;
    public AudioClip hurt, death, injured;
    private bool isPlayingBreath = false;
    

    private float timeSurvived;
    private float countdownTimer = 90f; // Countdown starts at 60 seconds
    private bool isAlive = true;  // To check if the player is alive

    void Awake()
    {
        // Assign UI elements safely
        healthUI = GameObject.Find("Health Text")?.GetComponent<TextMeshProUGUI>();
        shieldUI = GameObject.Find("Shield Text")?.GetComponent<TextMeshProUGUI>();
        //timeUI = GameObject.Find("Time Text")?.GetComponent<TextMeshProUGUI>();
        timerUI = GameObject.Find("Timer Text")?.GetComponent<TextMeshProUGUI>();
        shieldParent = GameObject.Find("Shield Parent");

        // Debugging in case objects aren't found
        if (!healthUI) Debug.LogError("Health UI not found!");
        if (!shieldUI) Debug.LogError("Shield UI not found!");
        if (!timeUI) Debug.LogError("Time UI not found!");
        if (!timerUI) Debug.LogError("Timer UI not found!");
    }

    void Start()
    {
        health = 100;
        maxHealth = 100;
        shield = 0;
        timeSurvived = 0f;

        // Initialize UI text
        if (timerUI) timerUI.SetText("Timer: 90.0");
        if (healthUI) healthUI.SetText(health + " / " + maxHealth);
    }

    void Update()
    {

        if (isAlive)
        {

            timeSurvived += Time.deltaTime;  // Increment time while alive
            if (timeUI) timeUI.SetText("Time: " + Mathf.FloorToInt(timeSurvived)); // Update UI

            // Countdown logic
            if (countdownTimer > 0)
            {
                countdownTimer -= Time.deltaTime;
                countdownTimer = Mathf.Max(countdownTimer, 0); // Ensures it doesn't go below 0

                if (timerUI) timerUI.SetText("Timer: " + countdownTimer.ToString("F1")); // Display with two decimal places
            }
            else
            {
                EndGame();  
            }
        }

        // Update health display independently
        if (healthUI) healthUI.SetText(health + " / " + maxHealth);

       /* // Shield logic
        if (shield > 0)
        {
            shieldParent.SetActive(true);
            if (shieldUI) shieldUI.SetText(shield + "");
        }
        else
        {
            shieldParent.SetActive(false);
            shield = 0;
        }
       */
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

        if (health < 40 && isPlayingBreath == false)
        {
            isPlayingBreath = true;
            source.clip = injured;
            source.loop = true;
            source.Play();
        }

        health = Mathf.Max(health, 0);  // Ensures health doesn't go below 0
        if (health <= 0)
        {
            source.Stop();
            Die();
        }
    }

    private void Die()
    {
        source.PlayOneShot(death);
        Debug.Log("Player has died!");
        deathText.SetActive(true);
        crosshair.SetActive(false);
        isAlive = false; // Stop the timers
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        if (timeUI) timeUI.SetText("Time: " + Mathf.FloorToInt(timeSurvived)); // Set final time
        if (projectileScript != null)
        {
            if (scoreUI) scoreUI.SetText("Score: " + projectileScript.score);
        }
        else if (gpProjectileScript != null)
        {
            if (scoreUI) scoreUI.SetText("Score: " + gpProjectileScript.score);
        }
        else
        {
            if (scoreUI) scoreUI.SetText("Score: " + audioProjectileScript.score);
        }
        //if (timerUI) timerUI.SetText("Timer: 00.00"); // Stop the countdown at 0


        // Pause the game
        Time.timeScale = 0;
    }
    private void EndGame()
    {
        Debug.Log("Timer Ended");
        deathText.SetActive(true);
        crosshair.SetActive(false);
        deathUI.SetText("Game Over!");
        isAlive = false; // Stop the timers
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        // Manually finalize the time before setting text
        timeSurvived += Time.deltaTime;
        if (timeUI) timeUI.SetText("Time: " + Mathf.FloorToInt(timeSurvived));
        if (projectileScript != null)
        {
            if (scoreUI) scoreUI.SetText("Score: " + projectileScript.score);
        }
        else if (gpProjectileScript != null)
        {
            if (scoreUI) scoreUI.SetText("Score: " + gpProjectileScript.score);
        }
        else
        {
            if (scoreUI) scoreUI.SetText("Score: " + audioProjectileScript.score);
        }
        if (timerUI) timerUI.SetText(""); // Stop the countdown at 0

        // Pause the game
        Time.timeScale = 0;
    }
}
