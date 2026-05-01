// Assets/Scripts/Player/PlayerCombatController.cs
// 玩家战斗控制：轻击(左键)、重击(右键)、格挡(Q)、闪避(Space)
// 依赖：PlayerStats、AttackArea、CharacterController

using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCombatController : MonoBehaviour
{
    [Header("伤害设置")]
    public float lightAttackDamage = 20f;   // 轻击伤害
    public float heavyAttackDamage = 40f;   // 重击伤害

    [Header("耐力消耗")]
    public float lightAttackStamina = 10f;
    public float heavyAttackStamina = 25f;
    public float dodgeStamina = 20f;

    [Header("攻击冷却")]
    public float lightAttackCooldown = 0.6f;
    public float heavyAttackCooldown = 1.2f;
    public float dodgeCooldown = 0.8f;

    [Header("闪避")]
    public float dodgeDistance = 4f;        // 闪避距离
    public float dodgeDuration = 0.25f;     // 闪避持续时间

    [Header("引用 - 把WeaponHitbox拖进来")]
    public AttackArea attackArea;           // 拖入WeaponHitbox物体上的AttackArea

    // 内部引用
    private PlayerStats stats;
    private CharacterController cc;

    // 计时器
    private float lightAttackTimer = 0f;
    private float heavyAttackTimer = 0f;
    private float dodgeTimer = 0f;

    // 状态
    private bool isBlocking = false;
    private bool isDodging = false;
    private float dodgeTimeLeft = 0f;
    private Vector3 dodgeDirection;

    // 暴露给其他脚本查询
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

        // 冷却计时
        lightAttackTimer -= Time.deltaTime;
        heavyAttackTimer -= Time.deltaTime;
        dodgeTimer -= Time.deltaTime;

        // 闪避移动
        if (isDodging)
        {
            dodgeTimeLeft -= Time.deltaTime;
            cc.Move(dodgeDirection * dodgeDistance / dodgeDuration * Time.deltaTime);
            if (dodgeTimeLeft <= 0f)
                isDodging = false;
            return; // 闪避中不处理其他输入
        }

        // 格挡（按住Q）
        isBlocking = Input.GetKey(KeyCode.Q) && !isDodging;

        // 格挡时不能攻击
        if (isBlocking) return;

        // 轻击（鼠标左键）
        if (Input.GetMouseButtonDown(0) && lightAttackTimer <= 0f)
        {
            if (stats.UseStamina(lightAttackStamina))
            {
                lightAttackTimer = lightAttackCooldown;
                DoAttack(lightAttackDamage);
                Debug.Log("轻击！");
            }
            else
            {
                Debug.Log("耐力不足，无法轻击");
            }
        }

        // 重击（鼠标右键）
        if (Input.GetMouseButtonDown(1) && heavyAttackTimer <= 0f)
        {
            if (stats.UseStamina(heavyAttackStamina))
            {
                heavyAttackTimer = heavyAttackCooldown;
                DoAttack(heavyAttackDamage);
                Debug.Log("重击！");
            }
            else
            {
                Debug.Log("耐力不足，无法重击");
            }
        }

        // 闪避（Space）
        if (Input.GetKeyDown(KeyCode.Space) && dodgeTimer <= 0f)
        {
            if (stats.UseStamina(dodgeStamina))
            {
                dodgeTimer = dodgeCooldown;
                StartDodge();
                Debug.Log("闪避！");
            }
        }
    }

    void DoAttack(float damage)
    {
        if (attackArea != null)
            attackArea.Activate(damage);
    }

    void StartDodge()
    {
        isDodging = true;
        dodgeTimeLeft = dodgeDuration;

        // 闪避方向：有输入就往输入方向闪，没输入就往后闪
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0f, v);

        if (input.magnitude > 0.1f)
        {
            // 转换为摄像机朝向
            CameraController cam = Camera.main?.GetComponent<CameraController>();
            if (cam != null)
                dodgeDirection = (cam.flatForward * v + cam.flatRight * h).normalized;
            else
                dodgeDirection = transform.forward;
        }
        else
        {
            dodgeDirection = -transform.forward; // 向后闪
        }
    }
}
