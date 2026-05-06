// Assets/Scripts/Combat/CharacterStats.cs
using UnityEngine;
using UnityEngine.Events;

public class CharacterStats : MonoBehaviour
{
    [Header("属性")]
    public float maxHealth = 100f;
    public float attackPower = 20f;
    public float defensePower = 5f;

    [HideInInspector] public float currentHealth;

    public UnityEvent<float, float> onHealthChanged;
    public UnityEvent onDeath;

    protected bool isDead = false;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        float final = Mathf.Max(1f, damage - defensePower);
        currentHealth -= final;
        currentHealth = Mathf.Max(0f, currentHealth);

        onHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log(gameObject.name + " 受到伤害: " + final + " 剩余血量: " + currentHealth);

        if (currentHealth <= 0f)
            Die();
    }

    protected virtual void Die()
    {
        if (isDead) return;  // 防止重复调用

        isDead = true;
        onDeath?.Invoke();
        Debug.Log(gameObject.name + " 死亡");
    }

    public bool IsDead() => isDead;
    public float GetHealthPercent() => currentHealth / maxHealth;
}