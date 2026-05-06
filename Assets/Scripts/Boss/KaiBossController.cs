// Assets/Scripts/Boss/KaiBossController.cs
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(KaiBossAI))]
[RequireComponent(typeof(Animator))]
public class KaiBossController : MonoBehaviour
{
    [Header("攻击判定")]
    public AttackArea attackArea;

    private CharacterStats stats;
    private KaiBossAI ai;

    void Start()
    {
        stats = GetComponent<CharacterStats>();
        ai = GetComponent<KaiBossAI>();
    }

    // 由动画事件调用
    public void OnAttackHit()
    {
        if (attackArea != null)
            attackArea.Activate(stats.attackPower);
    }
}