using UnityEngine;

public sealed class SpikeTrap : MonoBehaviour
{
    public string gameOverDetail = "Impaled by spikes";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>())
            GameOverPrompt.Show("GAME OVER", gameOverDetail);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<PlayerController>())
            GameOverPrompt.Show("GAME OVER", gameOverDetail);
    }
}
