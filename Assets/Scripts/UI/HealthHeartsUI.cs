using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 挂在 Canvas 下的空物体上：拖入 3 个心形 Image，根据 PlayerHealth 显示/隐藏。
/// </summary>
[DisallowMultipleComponent]
public sealed class HealthHeartsUI : MonoBehaviour
{
    [Header("引用")]
    [Tooltip("场景里带 PlayerHealth 的玩家；不填则在运行时 FindFirstObjectByType")]
    public PlayerHealth playerHealth;

    [Tooltip("从左到右三颗心，索引 0 对应第 1 格血")]
    public Image[] heartImages = new Image[3];

    private void OnEnable()
    {
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.onHealthChanged.AddListener(OnHealthChanged);
            OnHealthChanged(playerHealth.Current, playerHealth.maxHealth);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.onHealthChanged.RemoveListener(OnHealthChanged);
    }

    private void OnHealthChanged(int current, int max)
    {
        if (heartImages == null)
            return;

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null)
                continue;

            // 第 i 颗心：血量大于 i 时显示为「有」
            bool filled = i < current && i < max;
            heartImages[i].enabled = filled;
        }
    }
}
