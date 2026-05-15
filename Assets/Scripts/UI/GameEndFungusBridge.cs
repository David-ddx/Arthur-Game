using UnityEngine;

public class GameEndFungusBridge : MonoBehaviour
{
    public GameEndUI gameEndUI;

    public void ShowGameEndUI()
    {
        if (gameEndUI != null)
        {
            gameEndUI.ShowGameEndUI();
        }
        else
        {
            Debug.LogWarning("GameEndFungusBridge: GameEndUI Ã»ÓÐ°ó¶¨¡£");
        }
    }
}
