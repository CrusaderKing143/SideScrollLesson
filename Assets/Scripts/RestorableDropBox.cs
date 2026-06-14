using UnityEngine;

public sealed class RestorableDropBox : MonoBehaviour
{
    public int hitsToDrop = 5;
    public Color fadedColor = Color.gray;
    public Color restoredColor = Color.white;
    public float fallSpeed = 6f;
    public string gameOverDetail = "Crushed by the falling box";

    private int hitCount;
    private bool falling;
    private bool landed;

    private void Start()
    {
        GetComponent<SpriteRenderer>().color = fadedColor;
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Update()
    {
        if (falling)
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    public void HitByBullet(PlayerBullet bullet)
    {
        bullet.DestroyBullet();

        if (falling || landed)
            return;

        hitCount++;

        float colorT = (float)hitCount / hitsToDrop;
        GetComponent<SpriteRenderer>().color = Color.Lerp(fadedColor, restoredColor, colorT);

        if (hitCount >= hitsToDrop)
        {
            falling = true;
            GetComponent<SpriteRenderer>().color = restoredColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!falling)
            return;

        if (other.GetComponent<PlayerController>())
            GameOverPrompt.Show("GAME OVER", gameOverDetail);

        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            falling = false;
            landed = true;
            GetComponent<Collider2D>().isTrigger = false;
        }
    }
}
