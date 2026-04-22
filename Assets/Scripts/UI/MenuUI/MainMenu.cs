using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadSceneAsync("MainScene");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit button clicked");
    }
}
