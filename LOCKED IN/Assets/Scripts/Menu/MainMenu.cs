using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public GameObject guide, controls, menuUI, controlsExit;
    private bool guideEnabled = true, controlsEnabled = false, menuUIEnabled = false;
    private void Update()
    {
        if (guideEnabled && !controlsEnabled && !menuUIEnabled && Input.GetKeyDown(KeyCode.Space))
        {
            guideEnabled = false;
            guide.SetActive(false);
            controlsEnabled = true;
            controls.SetActive(true);
        }
        else if (!guideEnabled && controlsEnabled && !menuUIEnabled && Input.GetKeyDown(KeyCode.Space))
        {
            
            controlsEnabled = false;
            controls.SetActive(false);
            menuUIEnabled = true;
            menuUI.SetActive(true);
        }


    }
    public void PlayControlGame()
    {
        SceneManager.LoadScene("Control Scene");
    }
    public void PlayGPGame()
    {
        SceneManager.LoadScene("GPTransition");
    }
    public void PlayAudioGame()
    {
        SceneManager.LoadScene("Auditory Scene");
        Debug.Log("gameloaded");
    }
    public void PlayVisualGame()
    {
        SceneManager.LoadScene("VisualScene");
    }
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
    public void CheckControls()
    {

        controls.SetActive(true);
        controlsExit.SetActive(true);
        menuUI.SetActive(false);
    }
    public void ExitControls()
    {
        controls.SetActive(false);
        controlsExit.SetActive(false);
        menuUI.SetActive(true);
    }
}
