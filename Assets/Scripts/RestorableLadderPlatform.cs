using UnityEngine;

public sealed class RestorableLadderPlatform : MonoBehaviour
{
    public int hitsToActivate = 5;
    public Color fadedColor = Color.gray;
    public Color restoredColor = Color.white;
    public Transform pointA;
    public Transform pointB;
    public float moveDuration = 2f;

    private int hitCount;
    private bool active;
    private float timer;
    private Vector3 a;
    private Vector3 b;
    private PlayerController playerOnPlatform;

    private void Start()
    {
        a = transform.position;
        b = a;

        if (pointA)
            a = pointA.position;

        if (pointB)
            b = pointB.position;

        GetComponent<SpriteRenderer>().color = fadedColor;
        GetComponent<Collider2D>().isTrigger = true;
        transform.position = a;
    }

    private void Update()
    {
        Vector3 oldPosition = transform.position;

        if (active)
        {
            timer += Time.deltaTime;
            float t = Mathf.PingPong(timer / moveDuration, 1f);
            transform.position = Vector3.Lerp(a, b, t);
        }

        Vector3 platformMove = transform.position - oldPosition;

        if (playerOnPlatform)
            playerOnPlatform.transform.position += platformMove;
    }

    public void HitByBullet(PlayerBullet bullet)
    {
        bullet.DestroyBullet();
        hitCount++;

        float colorT = (float)hitCount / hitsToActivate;
        GetComponent<SpriteRenderer>().color = Color.Lerp(fadedColor, restoredColor, colorT);

        if (hitCount >= hitsToActivate)
        {
            active = true;
            GetComponent<Collider2D>().isTrigger = false;
            GetComponent<SpriteRenderer>().color = restoredColor;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        PlayerController player = collision.collider.GetComponent<PlayerController>();

        if (player && player.transform.position.y > transform.position.y)
            playerOnPlatform = player;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<PlayerController>())
            playerOnPlatform = null;
    }
}
