using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TransitionScreen : MonoBehaviour
{
    public TextMeshProUGUI objectiveText;
    public float displayTime = 3f; // Time before switching to game scene


    private void Awake()
    {
        Time.timeScale = 1;
    }
    void Start()
    {
        // Generate a random number between 18 and 27
        int enemiesToKill = Random.Range(18, 28); // Random.Range uses an exclusive upper bound for integers

        // Set the objective text
        objectiveText.text = $"Kill {enemiesToKill} enemies in 90 seconds!";

        // Start countdown to load game scene
        StartCoroutine(LoadGameScene());
    }

    IEnumerator LoadGameScene()
    {
        yield return new WaitForSeconds(displayTime);
        SceneManager.LoadScene("Gameplay Scene"); // Replace "GameScene" with your actual game scene name
    }
}