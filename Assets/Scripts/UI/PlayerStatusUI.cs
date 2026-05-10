// Assets/Scripts/UI/PlayerStatusUI.cs
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject playerStatusPanel;
    public Text playerNameText;
    public Image healthFillImage;
    public Image staminaFillImage;

    [Header("Player Settings")]
    public string playerName = "亚瑟";
    public PlayerStats playerStats;

    private void Awake()
    {
        if (playerStatusPanel != null)
        {
            playerStatusPanel.SetActive(true);
        }

        if (playerNameText != null)
        {
            playerNameText.text = playerName;
        }
    }

    private IEnumerator Start()
    {
        // 等一帧，确保 PlayerStats.Start() 已经完成初始化
        yield return null;

        BindPlayerStats();
        RefreshAllBars();
    }

    private void Update()
    {
        // 主动刷新，防止某些地方改了数值但没有触发 UnityEvent
        RefreshAllBars();
    }

    private void OnDestroy()
    {
        UnbindPlayerStats();
    }

    private void BindPlayerStats()
    {
        if (playerStats == null)
        {
            Debug.LogWarning("PlayerStatusUI: 没有绑定 PlayerStats，玩家血量条和耐力条不会更新。");
            return;
        }

        playerStats.onHealthChanged.AddListener(UpdateHealthBar);
        playerStats.onStaminaChanged.AddListener(UpdateStaminaBar);
        playerStats.onDeath.AddListener(OnPlayerDeath);
    }

    private void UnbindPlayerStats()
    {
        if (playerStats == null)
        {
            return;
        }

        playerStats.onHealthChanged.RemoveListener(UpdateHealthBar);
        playerStats.onStaminaChanged.RemoveListener(UpdateStaminaBar);
        playerStats.onDeath.RemoveListener(OnPlayerDeath);
    }

    private void RefreshAllBars()
    {
        if (playerStats == null)
        {
            return;
        }

        UpdateHealthBar(playerStats.currentHealth, playerStats.maxHealth);
        UpdateStaminaBar(playerStats.currentStamina, playerStats.maxStamina);
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthFillImage == null)
        {
            return;
        }

        if (maxHealth <= 0f)
        {
            healthFillImage.fillAmount = 0f;
            return;
        }

        float healthPercent = currentHealth / maxHealth;
        healthFillImage.fillAmount = Mathf.Clamp01(healthPercent);
    }

    private void UpdateStaminaBar(float currentStamina, float maxStamina)
    {
        if (staminaFillImage == null)
        {
            return;
        }

        if (maxStamina <= 0f)
        {
            staminaFillImage.fillAmount = 0f;
            return;
        }

        float staminaPercent = currentStamina / maxStamina;
        staminaFillImage.fillAmount = Mathf.Clamp01(staminaPercent);
    }

    private void OnPlayerDeath()
    {
        if (playerStats != null)
        {
            UpdateHealthBar(0f, playerStats.maxHealth);
        }
    }
}
