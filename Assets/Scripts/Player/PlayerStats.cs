// Assets/Scripts/Player/PlayerStats.cs
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [Header("基础属性")]
    public float maxHealth = 150f;
    public float maxStamina = 100f;
    public float attackPower = 20f;
    public float defensePower = 5f;

    [Header("耐力回复")]
    public float staminaRegenRate = 15f;
    public float staminaRegenDelay = 1f;

    [Header("死亡设置")]
    public float reloadDelay = 3f;
    public GameObject deathUI; // 把 DeathCanvas 拖进来

    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentStamina;

    public UnityEvent<float, float> onHealthChanged;
    public UnityEvent<float, float> onStaminaChanged;
    public UnityEvent onDeath;

    private float staminaRegenTimer = 0f;
    private bool isDead = false;
    private Animator anim;

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        anim = GetComponent<Animator>();
        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        if (deathUI != null)
            deathUI.SetActive(false);
        else
            Debug.LogWarning("PlayerStats: Death UI 没有拖进来。");

        if (anim != null)
            anim.SetBool("Dead", false);
        else
            Debug.LogWarning("PlayerStats: 没找到 Animator。");

        onHealthChanged?.Invoke(currentHealth, maxHealth);
        onStaminaChanged?.Invoke(currentStamina, maxStamina);

        Debug.Log("玩家初始血量 = " + currentHealth);
    }

    void Update()
    {
        if (isDead) return;

        if (currentStamina < maxStamina)
        {
            staminaRegenTimer -= Time.deltaTime;

            if (staminaRegenTimer <= 0f)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina);
                onStaminaChanged?.Invoke(currentStamina, maxStamina);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        float finalDamage = Mathf.Max(0, damage - defensePower);
        currentHealth -= finalDamage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log("PlayerStats 玩家受到伤害: " + finalDamage + " 剩余血量: " + currentHealth);

        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public bool UseStamina(float amount)
    {
        if (isDead) return false;
        if (currentStamina < amount) return false;

        currentStamina -= amount;
        staminaRegenTimer = staminaRegenDelay;
        onStaminaChanged?.Invoke(currentStamina, maxStamina);
        return true;
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log("玩家死亡 Die() 被调用");

        if (anim != null)
        {
            Debug.Log("设置玩家 Dead = true");
            anim.SetFloat("Speed", 0f);
            anim.SetBool("Dead", true);
        }

        if (deathUI != null)
        {
            deathUI.SetActive(true);
            Debug.Log("死亡UI已显示：" + deathUI.name);
        }
        else
        {
            Debug.LogWarning("PlayerStats: Death UI 没有拖进来！");
        }

        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
            controller.enabled = false;

        PlayerCombatController combat = GetComponent<PlayerCombatController>();
        if (combat != null)
            combat.enabled = false;

        PlayerAnimController animController = GetComponent<PlayerAnimController>();
        if (animController != null)
            animController.enabled = false;

        onDeath?.Invoke();

        Invoke(nameof(ReloadScene), reloadDelay);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsDead()
    {
        return isDead;
    }

    [ContextMenu("Test Player Death")]
    void TestPlayerDeath()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("请先点 Play 再测试玩家死亡。");
            return;
        }

        currentHealth = 0f;
        Die();
    }
}
