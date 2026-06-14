using UnityEngine;

public sealed class FireMachine : MonoBehaviour
{
    public GameObject smokeObject;
    public GameObject fireObject;
    public SpriteRenderer FireSprite;
    public float waitTime = 1.8f;
    public float smokeTime = 0.8f;
    public float fireTime = 1.2f;
    public string gameOverDetail = "Burned by the flame trap";

    private float timer;
    private int state;

    private void Start()
    {
        fireObject = FireSprite.gameObject;
        BoxCollider2D fireCollider = fireObject.AddComponent<BoxCollider2D>();
        fireCollider.isTrigger = true;
        fireCollider.size = FireSprite.sprite.bounds.size;
        fireCollider.offset = FireSprite.sprite.bounds.center;

        FireMachineHitBox hitBox = fireObject.AddComponent<FireMachineHitBox>();
        hitBox.fireMachine = this;

        timer = waitTime;
        smokeObject.SetActive(false);
        fireObject.SetActive(false);
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            state++;

            if (state == 1)
            {
                smokeObject.SetActive(true);
                fireObject.SetActive(false);
                timer = smokeTime;
            }
            else if (state == 2)
            {
                smokeObject.SetActive(false);
                fireObject.SetActive(true);
                timer = fireTime;
            }
            else
            {
                state = 0;
                smokeObject.SetActive(false);
                fireObject.SetActive(false);
                timer = waitTime;
            }
        }
    }

    public void TouchFire(Collider2D other)
    {
        if (fireObject.activeSelf && other.GetComponent<PlayerController>())
            GameOverPrompt.Show("GAME OVER", gameOverDetail);
    }
}

public sealed class FireMachineHitBox : MonoBehaviour
{
    public FireMachine fireMachine;

    private void OnTriggerEnter2D(Collider2D other)
    {
        fireMachine.TouchFire(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        fireMachine.TouchFire(other);
    }
}
