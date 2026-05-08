using UnityEngine;

[System.Serializable]
public class QuestData
{
    [Header("Quest Info")]
    public string questId;

    public string title;

    [TextArea(2, 4)]
    public string description;
}
