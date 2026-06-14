using UnityEngine;

public sealed class YellowJarWeaponUnlock : MonoBehaviour
{
    public GameObject jarObject;
    public SpriteRenderer armRenderer;
    public Sprite unlockedArmSprite;
    public ShootController shootController;
    public SpriteRenderer armSpriteRenderer;
    public Sprite yellowArmSprite;

    private void Start()
    {
        jarObject = gameObject;
        armRenderer = armSpriteRenderer;
        unlockedArmSprite = yellowArmSprite;
        shootController.canShoot = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>())
        {
            armRenderer.sprite = unlockedArmSprite;
            shootController.canShoot = true;
            jarObject.SetActive(false);
        }
    }
}
