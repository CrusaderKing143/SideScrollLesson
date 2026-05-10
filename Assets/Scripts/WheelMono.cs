using UnityEngine;

public class WheelMono : MonoBehaviour
{
    [Header("伤害")]
    [Min(1)]
    [Tooltip("碰到带 Player 标签且含 PlayerHealth 的对象时扣除的生命")]
    public int damageOnHit = 1;

    [Header("旋转")]
    [Tooltip("每秒旋转角度（Z 轴，2D 平面内转动）")]
    public float rotateSpeedDegPerSec = 100f;

    [Header("上下往返")]
    [Tooltip("相对起始位置的总行程（世界空间 Y，单位：米）。例如 2 表示从起点下 1m 到上 1m")]
    public float verticalTravel = 2f;

    [Tooltip("完成一次上下往返（到底再回原高度）所需时间（秒），越小越快")]
    public float verticalCycleDuration = 2f;

    private float _startY;

    private void Awake()
    {
        _startY = transform.position.y;
    }

    private void Update()
    {
        transform.Rotate(0f, 0f, rotateSpeedDegPerSec * Time.deltaTime);

        if (verticalTravel <= 0f || verticalCycleDuration <= 0f)
            return;

        float half = verticalTravel * 0.5f;
        // PingPong 在 t 增加 2*L 时走完 0→L→0；令周期 = verticalCycleDuration ⇒ 乘数 = 2L/T
        float pingArg = Time.time * (2f * verticalTravel / verticalCycleDuration);
        float y = _startY + Mathf.PingPong(pingArg, verticalTravel) - half;
        Vector3 p = transform.position;
        p.y = y;
        transform.position = p;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider != null)
            TryDamagePlayer(collision.collider);
    }

    private void TryDamagePlayer(Collider2D other)
    {
        if (other == null || !other.CompareTag("Player"))
            return;

        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
        if (health == null)
            health = other.GetComponent<PlayerHealth>();
        if (health == null)
            return;

        health.TakeDamage(damageOnHit);
    }
}
