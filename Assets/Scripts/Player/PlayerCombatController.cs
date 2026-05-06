// Assets/Scripts/Player/PlayerCombatController.cs
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCombatController : MonoBehaviour
{
    [Header("伤害设置")]
    public float lightAttackDamage = 20f;
    public float heavyAttackDamage = 40f;

    [Header("耐力消耗")]
    public float lightAttackStamina = 10f;
    public float heavyAttackStamina = 25f;
    public float dodgeStamina = 20f;

    [Header("攻击冷却")]
    public float lightAttackCooldown = 0.6f;
    public float heavyAttackCooldown = 1.2f;
    public float dodgeCooldown = 0.8f;

    [Header("闪避")]
    public float dodgeDistance = 4f;
    public float dodgeDuration = 0.25f;

    [Header("引用")]
    public AttackArea attackArea;

    private PlayerStats stats;
    private CharacterController cc;
    private float lightAttackTimer = 0f;
    private float heavyAttackTimer = 0f;
    private float dodgeTimer = 0f;
    private bool isBlocking = false;
    private bool isDodging = false;
    private float dodgeTimeLeft = 0f;
    private Vector3 dodgeDirection;
    private float pendingAttackDamage = 0f;

    // 战斗是否启用（默认禁用，拾取武器后启用）
    [HideInInspector] public bool isCombatEnabled = false;

    public bool IsBlocking => isBlocking;
    public bool IsDodging => isDodging;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        cc = GetComponent<CharacterController>();

        if (attackArea == null)
            Debug.LogError("PlayerCombatController：请把WeaponHitbox上的AttackArea拖进来！");
    }

    void Update()
    {
        if (stats.IsDead()) return;

        lightAttackTimer -= Time.deltaTime;
        heavyAttackTimer -= Time.deltaTime;
        dodgeTimer -= Time.deltaTime;

        if (isDodging)
        {
            dodgeTimeLeft -= Time.deltaTime;
            cc.Move(dodgeDirection * dodgeDistance / dodgeDuration * Time.deltaTime);
            if (dodgeTimeLeft <= 0f)
                isDodging = false;
            return;
        }

        isBlocking = Input.GetKey(KeyCode.Q) && !isDodging;
        if (isBlocking) return;

        // 战斗未启用时不能攻击
        if (!isCombatEnabled) return;

        if (Input.GetMouseButtonDown(0) && lightAttackTimer <= 0f)
        {
            if (stats.UseStamina(lightAttackStamina))
            {
                lightAttackTimer = lightAttackCooldown;
                DoAttack(lightAttackDamage, false);
            }
        }

        if (Input.GetMouseButtonDown(1) && heavyAttackTimer <= 0f)
        {
            if (stats.UseStamina(heavyAttackStamina))
            {
                heavyAttackTimer = heavyAttackCooldown;
                DoAttack(heavyAttackDamage, true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && dodgeTimer <= 0f)
        {
            if (stats.UseStamina(dodgeStamina))
            {
                dodgeTimer = dodgeCooldown;
                StartDodge();
            }
        }
    }

    void DoAttack(float damage, bool isHeavy)
    {
        pendingAttackDamage = damage;

        PlayerAnimController animController = GetComponent<PlayerAnimController>();
        if (animController != null)
        {
            if (isHeavy)
                animController.TriggerHeavyAttack();
            else
                animController.TriggerLightAttack();
        }
    }

    public void OnAttackHit()
    {
        if (attackArea != null && isCombatEnabled)
            attackArea.Activate(pendingAttackDamage);
    }

    void StartDodge()
    {
        isDodging = true;
        dodgeTimeLeft = dodgeDuration;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0f, v);

        if (input.magnitude > 0.1f)
        {
            CameraController cam = Camera.main?.GetComponent<CameraController>();
            if (cam != null)
                dodgeDirection = (cam.flatForward * v + cam.flatRight * h).normalized;
            else
                dodgeDirection = transform.forward;
        }
        else
        {
            dodgeDirection = -transform.forward;
        }
    }

    // 由WeaponPickup调用
    public void EnableCombat()
    {
        isCombatEnabled = true;
        Debug.Log("战斗系统已解锁！");
    }

    public void DisableCombat()
    {
        isCombatEnabled = false;
    }
}