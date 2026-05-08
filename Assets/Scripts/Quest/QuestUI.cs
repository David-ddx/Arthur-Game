using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject questPanel;
    public Text titleText;
    public Text descriptionText;

    private void Start()
    {
        HideQuest();
    }

    public void ShowQuest(QuestData quest)
    {
        if (questPanel != null)
        {
            questPanel.SetActive(true);
        }

        if (titleText != null)
        {
            titleText.text = quest.title;
        }

        if (descriptionText != null)
        {
            descriptionText.text = quest.description;
        }
    }

    public void HideQuest()
    {
        if (questPanel != null)
        {
            questPanel.SetActive(false);
        }
    }
}
