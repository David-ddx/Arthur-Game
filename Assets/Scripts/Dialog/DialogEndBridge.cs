using UnityEngine;

public class DialogEndBridge : MonoBehaviour
{
    public void NotifyTrainingDialogComplete()
    {
        GameObject managerObject = GameObject.Find("TrainingGroundManager");

        if (managerObject == null)
        {
            Debug.LogError("没有找到名为 TrainingGroundManager 的物体，请检查 Hierarchy 里的名字是否正确。");
            return;
        }

        TrainingGroundManager manager = managerObject.GetComponent<TrainingGroundManager>();

        if (manager == null)
        {
            Debug.LogError("TrainingGroundManager 物体上没有挂 TrainingGroundManager.cs 脚本。");
            return;
        }

        manager.OnDialogComplete();

        Debug.Log("对话结束：已调用 TrainingGroundManager.OnDialogComplete()");
    }
}
