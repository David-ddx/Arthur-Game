// Assets/Scripts/Boss/BossPhaseTransition.cs
// Boss阶段切换：血量50%触发狂暴，可以播放特效、改变颜色等

using UnityEngine;

public class BossPhaseTransition : MonoBehaviour
{
    [Header("阶段设置")]
    public float phase2HealthPercent = 0.5f;
    public GameObject rageEffect;  // 狂暴特效（可选）

    private CharacterStats stats;
    private bool hasEnteredPhase2 = false;

    void Start()
    {
        stats = GetComponent<CharacterStats>();
    }

    void Update()
    {
        if (hasEnteredPhase2) return;

        if (stats.GetHealthPercent() <= phase2HealthPercent)
        {
            EnterPhase2();
        }
    }

    void EnterPhase2()
    {
        hasEnteredPhase2 = true;
        Debug.Log("Boss进入第二阶段！");

        // 播放特效
        if (rageEffect != null)
            rageEffect.SetActive(true);

        // 可以在这里改变Boss颜色、播放音效等
    }
}
