using UnityEngine;

public class QuestFungusBridge : MonoBehaviour
{
    [Header("Training Ground")]
    public TrainingGroundManager trainingGroundManager;

    public void CompleteTalkEctorFirst()
    {
        CompleteQuest("TalkEctorFirst");
    }

    public void CompleteTalkMysteriousMan()
    {
        CompleteQuest("TalkMysteriousMan");
    }

    public void CompleteTalkEctorSecond()
    {
        CompleteQuest("TalkEctorSecond");
    }

    public void CompleteTalkKai()
    {
        CompleteQuest("TalkKai");

        if (trainingGroundManager != null)
        {
            trainingGroundManager.OnDialogComplete();
        }
        else
        {
            Debug.LogWarning("QuestFungusBridge: 没有绑定 TrainingGroundManager，凯不会自动走向擂台。");
        }
    }

    public void CompleteFindWeapon()
    {
        CompleteQuest("FindWeapon");
    }

    public void CompleteChallengeKai()
    {
        CompleteQuest("ChallengeKai");
    }

    private void CompleteQuest(string questId)
    {
        if (QuestManager.Instance == null)
        {
            Debug.LogWarning("QuestFungusBridge: 场景里没有 QuestManager。");
            return;
        }

        QuestManager.Instance.CompleteQuest(questId);
    }
}
