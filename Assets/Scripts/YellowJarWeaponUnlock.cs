using UnityEngine;

public sealed class YellowJarWeaponUnlock : MonoBehaviour
{
    public GameObject jarObject;
    public SpriteRenderer armRenderer;
    public Sprite unlockedArmSprite;
    public ShootController shootController;
    public SpriteRenderer armSpriteRenderer;
    public Sprite yellowArmSprite;
    public GameShootTeach gameShootTeach;

    private void Start()
    {
        jarObject = gameObject;
        armRenderer = armSpriteRenderer;
        unlockedArmSprite = yellowArmSprite;
        shootController.canShoot = false;

        EnsureGameShootTeach();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>())
        {
            armRenderer.sprite = unlockedArmSprite;
            shootController.canShoot = true;
            EnsureGameShootTeach();
            if (gameShootTeach)
                gameShootTeach.ShowGuide();
            jarObject.SetActive(false);
        }
    }

    private void EnsureGameShootTeach()
    {
        if (gameShootTeach)
            return;

        gameShootTeach = FindObjectOfType<GameShootTeach>();
        if (!gameShootTeach)
            gameShootTeach = new GameObject("GameShootTeach").AddComponent<GameShootTeach>();
    }
}
