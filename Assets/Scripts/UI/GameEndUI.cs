using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameEndUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameEndPanel;
    public Button endGameButton;

    [Header("Scene Settings")]
    public string menuSceneName = "MenuScene";

    [Header("Game Settings")]
    public bool pauseGameWhenShow = true;

    private void Start()
    {
        HideGameEndUI();

        if (endGameButton != null)
        {
            endGameButton.onClick.AddListener(OnEndGameButtonClicked);
        }
        else
        {
            Debug.LogWarning("GameEndUI: EndGameButton 没有绑定。");
        }
    }

    private void OnDestroy()
    {
        if (endGameButton != null)
        {
            endGameButton.onClick.RemoveListener(OnEndGameButtonClicked);
        }
    }

    public void ShowGameEndUI()
    {
        if (gameEndPanel != null)
        {
            gameEndPanel.SetActive(true);
        }

        if (pauseGameWhenShow)
        {
            Time.timeScale = 0f;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("显示游戏结束 UI。");
    }

    public void HideGameEndUI()
    {
        if (gameEndPanel != null)
        {
            gameEndPanel.SetActive(false);
        }
    }

    private void OnEndGameButtonClicked()
    {
        Time.timeScale = 1f;

        Debug.Log("点击结束游戏按钮，返回开始界面：" + menuSceneName);
        SceneManager.LoadScene(menuSceneName);
    }
}
