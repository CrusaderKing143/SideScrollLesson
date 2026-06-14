using UnityEngine;

public sealed class PlayerBullet : MonoBehaviour
{
    public float speed = 12f;
    public Vector3 moveDirection = Vector3.right;

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        RestorableDropBox box = other.GetComponent<RestorableDropBox>();
        RestorableLadderPlatform ladder = other.GetComponent<RestorableLadderPlatform>();

        if (box)
            box.HitByBullet(this);

        if (ladder)
            ladder.HitByBullet(this);
    }

    public void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
