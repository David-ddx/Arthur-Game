// Assets/Scripts/Combat/AttackArea.cs
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    [Header("攻击参数")]
    public float attackRadius = 2f;
    public LayerMask enemyLayer;

    private float pendingDamage = 0f;
    private bool isActive = false;
    private float activeTimer = 0f;
    public float activeDuration = 0.2f;

    public void Activate(float damage)
    {
        pendingDamage = damage;
        isActive = true;
        activeTimer = activeDuration;
    }

    public void Deactivate()
    {
        isActive = false;
    }

    void Update()
    {
        if (!isActive) return;

        activeTimer -= Time.deltaTime;

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, enemyLayer);

        foreach (Collider hit in hits)
        {
            // 先判断是不是玩家
            PlayerStats playerStats = hit.GetComponentInParent<PlayerStats>();

            if (playerStats != null && !playerStats.IsDead())
            {
                playerStats.TakeDamage(pendingDamage);
                isActive = false;
                return;
            }

            // 如果不是玩家，再判断是不是普通角色 / Boss
            CharacterStats characterStats = hit.GetComponentInParent<CharacterStats>();

            if (characterStats != null && !characterStats.IsDead())
            {
                characterStats.TakeDamage(pendingDamage);
                isActive = false;
                return;
            }
        }

        if (activeTimer <= 0f)
        {
            isActive = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
