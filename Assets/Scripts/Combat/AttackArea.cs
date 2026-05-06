// Assets/Scripts/Combat/AttackArea.cs
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    [Header("¹¥»÷²ÎÊý")]
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
            CharacterStats stats = hit.GetComponent<CharacterStats>();
            if (stats != null && !stats.IsDead())
            {
                stats.TakeDamage(pendingDamage);
                isActive = false;
                return;
            }
        }

        if (activeTimer <= 0f)
            isActive = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
