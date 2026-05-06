// Assets/Scripts/Scene/TrainingGroundManager.cs
using UnityEngine;

public class TrainingGroundManager : MonoBehaviour
{
    [Header("引用")]
    public KaiBossAI kaiBossAI;
    public PlayerCombatController playerCombat;
    public GameObject[] arenaWalls;  // 擂台空气墙数组

    [Header("状态")]
    public bool isBattleStarted = false;

    void Start()
    {
        // 禁用空气墙
        foreach (GameObject wall in arenaWalls)
        {
            if (wall != null)
                wall.SetActive(false);
        }
    }

    // 由董玳瑄的Fungus对话结束后调用
    public void OnDialogComplete()
    {
        Debug.Log("对话完成，凯开始走向擂台");
        if (kaiBossAI != null)
            kaiBossAI.StartWalkingToArena();
    }

    // 由ArenaTrigger调用（玩家进入擂台）
    public void OnPlayerEnterArena()
    {
        if (isBattleStarted) return;

        Debug.Log("玩家进入擂台，战斗开始！");
        isBattleStarted = true;

        // 启用凯的战斗AI
        if (kaiBossAI != null)
            kaiBossAI.StartFighting();

        // 激活空气墙
        foreach (GameObject wall in arenaWalls)
        {
            if (wall != null)
                wall.SetActive(true);
        }

        // 可以在这里通知董玳瑄的系统播放战斗BGM、显示Boss血条等
    }

    // 由凯的死亡事件调用
    public void OnKaiDefeated()
    {
        Debug.Log("凯被击败，战斗结束");

        // 禁用空气墙
        foreach (GameObject wall in arenaWalls)
        {
            if (wall != null)
                wall.SetActive(false);
        }

        // 通知董玳瑄的系统触发胜利UI/对话
        // QuestManager.Instance.CompleteQuest("DefeatKai");
    }
}
