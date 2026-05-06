// Assets/Scripts/Interaction/ArenaTrigger.cs
using UnityEngine;

public class ArenaTrigger : MonoBehaviour
{
    public TrainingGroundManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.OnPlayerEnterArena();
            // 触发后不销毁，因为可能需要检测玩家是否离开擂台
        }
    }
}
