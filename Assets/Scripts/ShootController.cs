using UnityEngine;
using UnityEngine.InputSystem;

public sealed class ShootController : MonoBehaviour
{
    public Sprite YellowBullet;
    public Sprite RedBullet;
    public Sprite BlueBullet;
    public Sprite GreenBullet;

    public Transform bulletSpawnPoint;
    public ArmAimController armAimController;
    public Transform aimPivot;

    [Header("瞄准 / 飞行")]
    [Tooltip("不指定则用 Camera.main")]
    public Camera shootCamera;

    [Tooltip("子弹速度（世界单位/秒）")]
    public float bulletSpeed = 12f;

    [Tooltip("几秒后销毁子弹，避免无限堆积")]
    public float bulletLifetime = 5f;

    [Tooltip("素材本身斜向时的 Z 角补正（度），叠在飞行方向上；例如 -37.45 让贴图与弹道对齐")]
    public float bulletVisualRotationOffsetZ = -37.45f;

    private GameObject SpawnBulletFromSprite(Sprite sprite, Vector2 direction, Vector2 spawnPosition)
    {
        if (sprite == null)
            return null;

        Vector2 dir = direction.sqrMagnitude > 1e-6f ? direction.normalized : Vector2.right;

        var go = new GameObject(sprite.name + "_Bullet");
        go.transform.position = spawnPosition;
        float flyAngleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        go.transform.rotation = Quaternion.Euler(0f, 0f, flyAngleDeg + bulletVisualRotationOffsetZ);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;

        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.linearVelocity = dir * bulletSpeed;

        Destroy(go, bulletLifetime);
        return go;
    }

    public void Shoot(string bulletColor)
    {
        if (bulletSpawnPoint == null)
            return;

        Sprite s = bulletColor switch
        {
            "Yellow" => YellowBullet,
            "Red" => RedBullet,
            "Blue" => BlueBullet,
            "Green" => GreenBullet,
            _ => null
        };

        if (s == null)
            return;

        ArmAimController aim = ResolveArmAimController();
        if (aim != null)
            aim.RefreshAim();

        Vector2 dir = GetShotDirection();
        SpawnBulletFromSprite(s, dir, bulletSpawnPoint.position);
    }

    private Vector2 GetShotDirection()
    {
        Vector2 spawnPosition = bulletSpawnPoint.position;
        Transform pivot = GetAimPivot();

        if (pivot != null)
        {
            Vector2 pivotToMuzzle = spawnPosition - (Vector2)pivot.position;
            if (pivotToMuzzle.sqrMagnitude > 1e-6f)
                return pivotToMuzzle;
        }

        if (bulletSpawnPoint.parent != null)
        {
            Vector2 parentToMuzzle = spawnPosition - (Vector2)bulletSpawnPoint.parent.position;
            if (parentToMuzzle.sqrMagnitude > 1e-6f)
                return parentToMuzzle;
        }

        Vector2 localRight = bulletSpawnPoint.TransformDirection(Vector3.right);
        if (localRight.sqrMagnitude > 1e-6f)
            return localRight;

        return GetMouseWorldOnPlane(bulletSpawnPoint) - spawnPosition;
    }

    private Transform GetAimPivot()
    {
        if (aimPivot != null)
            return aimPivot;

        ArmAimController aim = ResolveArmAimController();
        if (aim == null)
            return null;

        return aim.armPivot != null ? aim.armPivot : aim.transform;
    }

    private ArmAimController ResolveArmAimController()
    {
        if (armAimController != null)
            return armAimController;

        if (bulletSpawnPoint == null)
            return null;

        for (Transform current = bulletSpawnPoint; current != null; current = current.parent)
        {
            if (current.TryGetComponent(out ArmAimController aim))
            {
                armAimController = aim;
                return armAimController;
            }
        }

        return null;
    }

    private Vector2 GetMouseWorldOnPlane(Transform depthReference)
    {
        Camera cam = shootCamera != null ? shootCamera : Camera.main;
        if (cam == null || depthReference == null)
            return depthReference != null ? depthReference.position : Vector2.zero;

        Vector3 screen = ReadMouseScreenPosition();
        screen.z = cam.WorldToScreenPoint(depthReference.position).z;
        return cam.ScreenToWorldPoint(screen);
    }

    private static Vector3 ReadMouseScreenPosition()
    {
        if (Mouse.current != null)
            return Mouse.current.position.ReadValue();
        return Input.mousePosition;
    }

    private void Update()
    {
        if (!WasLeftClickPressedThisFrame())
            return;

        Shoot("Yellow");
    }

    private static bool WasLeftClickPressedThisFrame()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;
        return Input.GetMouseButtonDown(0);
    }
}
