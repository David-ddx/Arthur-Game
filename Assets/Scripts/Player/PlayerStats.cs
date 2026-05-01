// Assets/Scripts/Player/PlayerStats.cs
// 玩家属性脚本：管理血量、耐力、攻击力、防御力
// 其他脚本通过 GetComponent<PlayerStats>() 来读取和修改属性

using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    [Header("基础属性")]
    public float maxHealth = 100f;       // 最大血量
    public float maxStamina = 100f;      // 最大耐力
    public float attackPower = 20f;      // 攻击力
    public float defensePower = 5f;      // 防御力（受到伤害时减免）

    [Header("耐力回复")]
    public float staminaRegenRate = 15f; // 每秒回复耐力量
    public float staminaRegenDelay = 1f; // 用完耐力后多少秒才开始回复

    // 当前值（运行时变化）
    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentStamina;

    // 事件：UI脚本监听这些事件来更新血条、耐力条
    public UnityEvent<float, float> onHealthChanged;   // 参数：当前血量, 最大血量
    public UnityEvent<float, float> onStaminaChanged;  // 参数：当前耐力, 最大耐力
    public UnityEvent onDeath;                          // 死亡事件

    private float staminaRegenTimer = 0f; // 耐力回复计时器
    private bool isDead = false;

    void Start()
    {
        // 游戏开始时，当前值等于最大值
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    void Update()
    {
        // 耐力自动回复逻辑
        if (currentStamina < maxStamina)
        {
            staminaRegenTimer -= Time.deltaTime;
            if (staminaRegenTimer <= 0f)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina); // 不超过上限
                onStaminaChanged?.Invoke(currentStamina, maxStamina);
            }
        }
    }

    // ── 受到伤害 ──────────────────────────────────────────
    // 调用方式：stats.TakeDamage(30f);
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        // 防御减免
        float finalDamage = Mathf.Max(0, damage - defensePower);
        currentHealth -= finalDamage;
        currentHealth = Mathf.Max(0, currentHealth); // 不低于0

        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    // ── 回复血量 ──────────────────────────────────────────
    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // ── 消耗耐力 ──────────────────────────────────────────
    // 返回true表示成功消耗，返回false表示耐力不足
    public bool UseStamina(float amount)
    {
        if (currentStamina < amount) return false;

        currentStamina -= amount;
        staminaRegenTimer = staminaRegenDelay; // 重置回复计时器
        onStaminaChanged?.Invoke(currentStamina, maxStamina);
        return true;
    }

    // ── 死亡处理 ──────────────────────────────────────────
    private void Die()
    {
        isDead = true;
        onDeath?.Invoke();
        Debug.Log("玩家死亡");
        // 后续：DeathUI显示，由UIManager监听onDeath事件来处理
    }

    public bool IsDead() => isDead;
}
