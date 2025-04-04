using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;

public class VHealth : MonoBehaviour
{
    public ProjectileScript projectileScript;
    public GPProjectileScript gpProjectileScript;
    public int health;
    public TextMeshProUGUI healthUI;
    public TextMeshProUGUI shieldUI, timeUI, scoreUI, timerUI, deathUI;
    public UnityEngine.UI.Image shieldImage;
    public int maxHealth;
    public int shield;
    public GameObject shieldParent;
    public GameObject deathText;
    public AudioSource source;
    public AudioClip hurt, death, injured;
    public float intensity;

    private Coroutine passiveBloodCoroutine;
    public PostProcessVolume pPVolume;
    Vignette vignette;
    private bool isFadingUp = true;
    private bool isPulsing = false;

    private float timeSurvived;
    private float countdownTimer = 90f; // Countdown starts at 60 seconds
    private bool isAlive = true;  // To check if the player is alive

    void Awake()
    {
        // Assign UI elements safely
        //healthUI = GameObject.Find("Health Text")?.GetComponent<TextMeshProUGUI>();
        shieldUI = GameObject.Find("Shield Text")?.GetComponent<TextMeshProUGUI>();
        // timeUI = GameObject.Find("Time Text")?.GetComponent<TextMeshProUGUI>();
        timerUI = GameObject.Find("Timer Text")?.GetComponent<TextMeshProUGUI>();
        shieldParent = GameObject.Find("Shield Parent");

        pPVolume.profile.TryGetSettings<Vignette>(out vignette);
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
        healthUI.SetText(health + " / " + maxHealth);
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
        healthUI.SetText(health + " / " + maxHealth);

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

            if (health >= 50) // Only play the regular blood effect above 50 HP
            {
                StartCoroutine(BloodEffect());
            }
            else if (!isPulsing) // If already below 50 HP, don't play regular effect, start pulsing instead
            {
                StartCoroutine(BloodEffectPulse());
            }
        }

        if (health < 40)
        {
            source.clip = injured;
            source.loop = true;
            source.Play();
        }

        health = Mathf.Max(health, 0); // Ensures health doesn't go below 0
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
        isAlive = false; // Stop the timers
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        if (timeUI) timeUI.SetText("Time: " + Mathf.FloorToInt(timeSurvived)); // Set final time
        if (projectileScript != null)
        {
            if (scoreUI) scoreUI.SetText("Score: " + projectileScript.score);
        }
        else
        {
            if (scoreUI) scoreUI.SetText("Score: " + gpProjectileScript.score);
        }

        //if (timerUI) timerUI.SetText("Timer: 00.00"); // Stop the countdown at 0


        // Pause the game
        Time.timeScale = 0;
    }
    private void EndGame()
    {
        Debug.Log("Timer Ended");
        deathText.SetActive(true);
        deathUI.SetText("Game Over!");
        isAlive = false; // Stop the timers

        if (timeUI) timeUI.SetText("Time: " + Mathf.FloorToInt(timeSurvived)); // Set final time
        if (scoreUI) scoreUI.SetText("Score: " + projectileScript.score);
        if (timerUI) timerUI.SetText(""); // Stop the countdown at 0

        // Pause the game
        Time.timeScale = 0;
    }
    private IEnumerator BloodEffect()
    {
        intensity = 0.6f;
        vignette.enabled.Override(true);
        vignette.intensity.Override(0.6f);

        while(intensity >0)
        {
            intensity -= 0.01f;

            if (intensity < 0) intensity = 0;
            
            vignette.intensity.Override(intensity);

            yield return new WaitForSeconds(0.1f);
        }
        vignette.enabled.Override(false);
        yield break;
    }
    private IEnumerator BloodEffectPulse()
    {
        //if (isPulsing) yield break; // Prevent multiple coroutines
        isPulsing = true;
        vignette.enabled.Override(true); // Ensure vignette is active

        float maxIntensity = 0.7f;
        float minIntensity = 0.2f;
        float fadeSpeed = 0.01f; // Adjust for smoothness

        while (health < 50)
        {
            // Fade in (0.2 -> 0.7)
             intensity = maxIntensity;

            // Fade out (0.7 -> 0.2)
            while (intensity > minIntensity)
            {
                intensity -= fadeSpeed;
                if (intensity < minIntensity) intensity = minIntensity;
                vignette.intensity.Override(intensity);
                yield return new WaitForSeconds(0.03f);
            }
            //vignette.enabled.Override(false);
            while (intensity < maxIntensity)
            {
                Debug.Log("Before " + intensity);
                intensity += fadeSpeed*2;
                Debug.Log(intensity);
                if (intensity > maxIntensity) intensity= maxIntensity;
                vignette.intensity.Override(intensity);
                yield return new WaitForSeconds(0.03f); // Smooth transition
            }
            //vignette.enabled.Override(false);

        }

        //vignette.intensity.Override(minIntensity); // Reset if health goes above 50
        //vignette.enabled.Override(false); // Turn off effect when stopping
       // isPulsing = false;
    }

}

