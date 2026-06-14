using UnityEngine;

public class WheelMono : MonoBehaviour
{
    public float rotateSpeed = 120f;
    public Transform pointA;
    public Transform pointB;
    public float moveDuration = 2f;
    public string gameOverDetail = "Hit by the spinning wheel";

    private Vector3 a;
    private Vector3 b;
    private float timer;

    private void Start()
    {
        a = transform.position;
        b = a;

        if (pointA)
            a = pointA.position;

        if (pointB)
            b = pointB.position;
    }

    private void Update()
    {
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);

        timer += Time.deltaTime;
        float t = Mathf.PingPong(timer / moveDuration, 1f);
        transform.position = Vector3.Lerp(a, b, t);
    }

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
