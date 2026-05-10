// Assets/Scripts/UI/BossHealthBarUI.cs
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject bossHealthPanel;
    public Text bossNameText;
    public Image healthFillImage;

    [Header("Boss Settings")]
    public string defaultBossName = "凯爵士";

    private CharacterStats currentBossStats;

    private void Start()
    {
        Hide();
    }

    private void OnDestroy()
    {
        UnbindBossEvents();
    }

    public void Show(CharacterStats bossStats)
    {
        Show(bossStats, defaultBossName);
    }

    public void Show(CharacterStats bossStats, string bossName)
    {
        if (bossStats == null)
        {
            Debug.LogWarning("BossHealthBarUI: bossStats 为空，无法显示 Boss 血条。");
            return;
        }

        UnbindBossEvents();

        currentBossStats = bossStats;

        if (bossHealthPanel != null)
        {
            bossHealthPanel.SetActive(true);
        }

        if (bossNameText != null)
        {
            bossNameText.text = bossName;
        }

        // 初始化显示当前血量
        UpdateHealthBar(currentBossStats.currentHealth, currentBossStats.maxHealth);

        // 监听血量变化和死亡事件
        currentBossStats.onHealthChanged.AddListener(UpdateHealthBar);
        currentBossStats.onDeath.AddListener(OnBossDeath);
    }

    public void Hide()
    {
        UnbindBossEvents();

        if (bossHealthPanel != null)
        {
            bossHealthPanel.SetActive(false);
        }
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthFillImage == null)
        {
            Debug.LogWarning("BossHealthBarUI: healthFillImage 没有绑定。");
            return;
        }

        if (maxHealth <= 0f)
        {
            healthFillImage.fillAmount = 0f;
            return;
        }

        float percent = currentHealth / maxHealth;
        healthFillImage.fillAmount = Mathf.Clamp01(percent);
    }

    private void OnBossDeath()
    {
        Hide();
    }

    private void UnbindBossEvents()
    {
        if (currentBossStats == null)
        {
            return;
        }

        currentBossStats.onHealthChanged.RemoveListener(UpdateHealthBar);
        currentBossStats.onDeath.RemoveListener(OnBossDeath);

        currentBossStats = null;
    }
}
