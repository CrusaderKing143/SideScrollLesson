using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 挂在玩家根物体上：当前生命与受伤；UI 通过事件刷新心形显示。
/// </summary>
[DisallowMultipleComponent]
public sealed class PlayerHealth : MonoBehaviour
{
    [Header("生命")]
    [Min(1)]
    public int maxHealth = 3;

    [Tooltip("受伤后多少秒内不再扣血，避免陷阱边缘连续触发")]
    public float invulnerabilityDuration = 0.75f;

    [Header("事件（可选）")]
    public UnityEvent<int, int> onHealthChanged;

    public UnityEvent onDied;

    public int Current { get; private set; }

    public bool IsDead => Current <= 0;

    private float _invulnUntil;

    private void Awake()
    {
        Current = maxHealth;
    }

    private void Start()
    {
        onHealthChanged?.Invoke(Current, maxHealth);
    }

    /// <summary>对玩家造成伤害；在无敌时间内会忽略。</summary>
    public void TakeDamage(int amount = 1)
    {
        if (amount <= 0 || IsDead)
            return;

        if (Time.time < _invulnUntil)
            return;

        Current = Mathf.Max(0, Current - amount);
        _invulnUntil = Time.time + invulnerabilityDuration;

        onHealthChanged?.Invoke(Current, maxHealth);

        if (Current <= 0)
            onDied?.Invoke();
    }

    /// <summary>重置为满血（例如读档、重生）。</summary>
    public void ResetToFull()
    {
        Current = maxHealth;
        _invulnUntil = 0f;
        onHealthChanged?.Invoke(Current, maxHealth);
    }
}
