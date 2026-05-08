// Assets/Scripts/Interaction/WeaponPickup.cs
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("提示UI")]
    public GameObject interactPrompt;      // 显示"按F拾取"的UI
    public Transform player;               // 玩家 Transform
    public float interactRange = 2f;       // 显示提示的距离

    [Header("武器挂手偏移（可微调）")]
    public Vector3 handPositionOffset = new Vector3(0.124f, 0.025f, -0.056f);
    public Vector3 handRotationOffset = new Vector3(89.972f, 0f, 179.102f);
    public Vector3 handScale = Vector3.one;

    private bool hasPickedUp = false;

    void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (player == null)
            player = GameObject.Find("Hero_Knight")?.transform;

        if (player == null)
            Debug.LogWarning("WeaponPickup: 未找到玩家 Transform！");
    }

    void Update()
    {
        if (hasPickedUp || interactPrompt == null || player == null)
            return;

        // 计算玩家与武器的距离
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactRange)
        {
            // 显示提示 UI
            interactPrompt.SetActive(true);

            // UI 面向玩家摄像机
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                interactPrompt.transform.LookAt(mainCam.transform);
                interactPrompt.transform.Rotate(0f, 180f, 0f);
            }

            // 按 F 拾取武器
            if (Input.GetKeyDown(KeyCode.F))
            {
                PickupWeapon();
            }
        }
        else
        {
            interactPrompt.SetActive(false);
        }
    }

    private void PickupWeapon()
    {
        hasPickedUp = true;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        Debug.Log("武器已拾取！");

        if (player != null)
        {
            // 解锁战斗
            PlayerCombatController combat = player.GetComponent<PlayerCombatController>();
            if (combat != null)
                combat.EnableCombat();
            else
                Debug.LogWarning("WeaponPickup: 玩家没有 PlayerCombatController！");

            // 挂武器到玩家手上 WeaponHolder
            Transform weaponHolder = player.Find("Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_R/Shoulder_R/Elbow_R/Hand_R/WeaponHolder");
            if (weaponHolder != null)
            {
                transform.SetParent(weaponHolder);
                transform.localPosition = handPositionOffset;
                transform.localRotation = Quaternion.Euler(handRotationOffset);
                transform.localScale = handScale;

                // 确保武器显示
                SetRendererEnabled(true);
            }
            else
            {
                Debug.LogWarning("WeaponPickup: 玩家没有 WeaponHolder，武器将隐藏");
                gameObject.SetActive(false);
            }
        }

        // 完成任务
        if (QuestManager.Instance != null)
        {
            if (QuestManager.Instance.IsCurrentQuest("FindWeapon"))
            {
                QuestManager.Instance.CompleteQuest("FindWeapon");
                Debug.Log("任务完成：FindWeapon");
            }
            else
            {
                Debug.LogWarning("WeaponPickup: 当前任务不是 FindWeapon");
            }
        }
        else
        {
            Debug.LogWarning("WeaponPickup: 场景里没有 QuestManager！");
        }
    }

    // 确保武器模型显示
    private void SetRendererEnabled(bool enabled)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
            r.enabled = enabled;
    }
}
