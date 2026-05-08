// Assets/Scripts/Interaction/WeaponPickup.cs
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("提示UI")]
    public GameObject interactPrompt;  // 显示"按F拾取"的UI

    private bool playerInRange = false;
    private bool hasPickedUp = false;

    void Start()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && !hasPickedUp && Input.GetKeyDown(KeyCode.F))
        {
            PickupWeapon();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPickedUp)
        {
            playerInRange = true;

            if (interactPrompt != null)
            {
                interactPrompt.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }
    }

    void PickupWeapon()
    {
        hasPickedUp = true;
        playerInRange = false;

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }

        // 解锁玩家战斗
        GameObject player = GameObject.Find("Hero_Knight");

        if (player != null)
        {
            PlayerCombatController combat = player.GetComponent<PlayerCombatController>();

            if (combat != null)
            {
                combat.EnableCombat();
            }
            else
            {
                Debug.LogWarning("WeaponPickup: Hero_Knight 上没有找到 PlayerCombatController。");
            }
        }
        else
        {
            Debug.LogWarning("WeaponPickup: 没有找到名为 Hero_Knight 的玩家物体。");
        }

        // 完成任务：在训练场寻找武器
        if (QuestManager.Instance != null)
        {
            if (QuestManager.Instance.IsCurrentQuest("FindWeapon"))
            {
                QuestManager.Instance.CompleteQuest("FindWeapon");
                Debug.Log("任务完成：FindWeapon，切换到下一个任务。");
            }
            else
            {
                Debug.LogWarning("WeaponPickup: 当前任务不是 FindWeapon，因此不会推进任务。当前任务是：" + QuestManager.Instance.CurrentQuestId);
            }
        }
        else
        {
            Debug.LogWarning("WeaponPickup: 场景里没有找到 QuestManager，无法完成 FindWeapon 任务。");
        }

        Debug.Log("拾取武器，战斗系统已解锁！");

        // 隐藏武器模型
        gameObject.SetActive(false);
    }
}
