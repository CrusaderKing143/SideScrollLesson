using UnityEngine;

/// <summary>
/// 挂在陷阱上：Collider2D 勾选 Is Trigger，玩家进入时扣血。
/// </summary>
[DisallowMultipleComponent]
public sealed class TrapDamage : MonoBehaviour
{
    [Min(1)]
    public int damage = 1;

    private void Reset()
    {
        var c = GetComponent<Collider2D>();
        if (c != null)
            c.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        var health = other.GetComponentInParent<PlayerHealth>();
        if (health == null)
            health = other.GetComponent<PlayerHealth>();
        if (health == null)
            return;

        health.TakeDamage(damage);
    }
}
