using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void PlayControlGame()
    {
        SceneManager.LoadScene("Control Scene");
    }
    public void PlayGPGame()
    {
        SceneManager.LoadScene("GPTransition");
    }
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
