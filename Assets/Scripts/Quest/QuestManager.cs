using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Quest List")]
    public QuestData[] quests;

    [Header("UI")]
    public QuestUI questUI;

    [Header("Start Settings")]
    public bool startQuestOnStart = true;
    public int startQuestIndex = 0;

    private int currentQuestIndex = -1;

    public QuestData CurrentQuest
    {
        get
        {
            if (currentQuestIndex < 0 || currentQuestIndex >= quests.Length)
            {
                return null;
            }

            return quests[currentQuestIndex];
        }
    }

    public string CurrentQuestId
    {
        get
        {
            QuestData quest = CurrentQuest;
            return quest == null ? "" : quest.questId;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (startQuestOnStart && quests != null && quests.Length > 0)
        {
            StartQuestByIndex(startQuestIndex);
        }
    }

    public void StartQuestByIndex(int index)
    {
        if (quests == null || quests.Length == 0)
        {
            Debug.LogWarning("QuestManager: 任务列表为空。");
            return;
        }

        if (index < 0 || index >= quests.Length)
        {
            Debug.LogWarning("QuestManager: 任务下标越界：" + index);
            return;
        }

        currentQuestIndex = index;
        RefreshQuestUI();
    }

    public void StartQuest(string questId)
    {
        int index = FindQuestIndex(questId);

        if (index == -1)
        {
            Debug.LogWarning("QuestManager: 找不到任务：" + questId);
            return;
        }

        currentQuestIndex = index;
        RefreshQuestUI();
    }

    public void CompleteCurrentQuest()
    {
        QuestData currentQuest = CurrentQuest;

        if (currentQuest == null)
        {
            Debug.LogWarning("QuestManager: 当前没有任务。");
            return;
        }

        CompleteQuest(currentQuest.questId);
    }

    public void CompleteQuest(string questId)
    {
        QuestData currentQuest = CurrentQuest;

        if (currentQuest == null)
        {
            Debug.LogWarning("QuestManager: 当前没有正在进行的任务。");
            return;
        }

        if (currentQuest.questId != questId)
        {
            Debug.LogWarning(
                "QuestManager: 尝试完成的任务不是当前任务。当前任务是：" +
                currentQuest.questId +
                "，收到的是：" +
                questId
            );
            return;
        }

        Debug.Log("任务完成：" + currentQuest.title);

        int nextIndex = currentQuestIndex + 1;

        if (nextIndex < quests.Length)
        {
            currentQuestIndex = nextIndex;
            RefreshQuestUI();
        }
        else
        {
            currentQuestIndex = -1;

            if (questUI != null)
            {
                questUI.HideQuest();
            }

            Debug.Log("所有线性任务已完成。");
        }
    }

    public bool IsCurrentQuest(string questId)
    {
        QuestData currentQuest = CurrentQuest;

        if (currentQuest == null)
        {
            return false;
        }

        return currentQuest.questId == questId;
    }

    private int FindQuestIndex(string questId)
    {
        for (int i = 0; i < quests.Length; i++)
        {
            if (quests[i].questId == questId)
            {
                return i;
            }
        }

        return -1;
    }

    private void RefreshQuestUI()
    {
        QuestData currentQuest = CurrentQuest;

        if (currentQuest == null)
        {
            return;
        }

        Debug.Log("当前任务：" + currentQuest.title);

        if (questUI != null)
        {
            questUI.ShowQuest(currentQuest);
        }
    }
}
