// Assets/Scripts/Player/PlayerAnimController.cs
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimController : MonoBehaviour
{
    private Animator anim;
    private PlayerController controller;
    private PlayerCombatController combat;
    private PlayerStats stats;

    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        combat = GetComponent<PlayerCombatController>();
        stats = GetComponent<PlayerStats>();

        // 强制初始化为Idle状态
        anim.SetFloat("Speed", 0f);
        anim.Play("Idle", 0, 0f);

        stats.onDeath.AddListener(OnDeath);
    }

    void Update()
    {
        if (stats.IsDead()) return;

        // 获取实际移动速度（不是velocity.magnitude，而是输入速度）
        float speed = controller.GetCurrentMoveSpeed();
        anim.SetFloat("Speed", speed);
    }

    public void TriggerLightAttack()
    {
        anim.SetTrigger("LightAttack");
    }

    public void TriggerHeavyAttack()
    {
        anim.SetTrigger("HeavyAttack");
    }

    void OnDeath()
    {
        anim.SetBool("Dead", true);
    }
}
