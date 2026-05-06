// Assets/Scripts/Player/PlayerStats.cs
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [Header("基础属性")]
    public float maxHealth = 100f;
    public float maxStamina = 100f;
    public float attackPower = 20f;
    public float defensePower = 5f;

    [Header("耐力回复")]
    public float staminaRegenRate = 15f;
    public float staminaRegenDelay = 1f;

    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentStamina;

    public UnityEvent<float, float> onHealthChanged;
    public UnityEvent<float, float> onStaminaChanged;
    public UnityEvent onDeath;

    private float staminaRegenTimer = 0f;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    void Update()
    {
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

        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
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
        if (currentStamina < amount) return false;

        currentStamina -= amount;
        staminaRegenTimer = staminaRegenDelay;
        onStaminaChanged?.Invoke(currentStamina, maxStamina);
        return true;
    }

    private void Die()
    {
        isDead = true;
        onDeath?.Invoke();
        Debug.Log("玩家死亡");

        // 禁用所有控制
        GetComponent<PlayerController>().enabled = false;
        GetComponent<PlayerCombatController>().enabled = false;

        // 3秒后重新加载场景
        Invoke("ReloadScene", 3f);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsDead() => isDead;
}