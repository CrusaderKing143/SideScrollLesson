using UnityEngine;

public sealed class ShootController : MonoBehaviour
{
    public bool canShoot = true;
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public Camera aimCamera;
    public float bulletSpeed = 12f;
    public float bulletVisualRotationOffsetZ = -37.45f;

    private void Start()
    {
    
    }

    private void Update()
    {
        if (canShoot && Input.GetMouseButtonDown(0))
            Shoot("");
    }

    public void SetShootingEnabled(bool enabled)
    {
        canShoot = enabled;
    }

    public void Shoot(string bulletColor)
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = -aimCamera.transform.position.z;

        Vector3 mouseWorld = aimCamera.ScreenToWorldPoint(mouse);
        Vector3 shootDirection = (mouseWorld - shootPoint.position).normalized;

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg + bulletVisualRotationOffsetZ;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, rotation);
        PlayerBullet playerBullet = bullet.GetComponent<PlayerBullet>();
        playerBullet.speed = bulletSpeed;
        playerBullet.moveDirection = shootDirection;
    }
}
