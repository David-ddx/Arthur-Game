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
            interactPrompt.SetActive(false);
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
                interactPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }

    void PickupWeapon()
    {
        hasPickedUp = true;
        playerInRange = false;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        // 解锁玩家战斗
        GameObject player = GameObject.Find("Hero_Knight");
        if (player != null)
        {
            PlayerCombatController combat = player.GetComponent<PlayerCombatController>();
            if (combat != null)
                combat.EnableCombat();
        }

        // 通知董玳瑄的QuestManager完成任务
        // QuestManager.Instance.CompleteQuest("FindWeapon");

        Debug.Log("拾取武器，战斗系统已解锁！");

        // 隐藏武器模型
        gameObject.SetActive(false);
    }
}