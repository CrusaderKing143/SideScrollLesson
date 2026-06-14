using UnityEngine;

public sealed class ArmAimController : MonoBehaviour
{
    public Transform armPivot;
    public Camera aimCamera;

    public Vector2 aimDirection = Vector2.right;

    private void Update()
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = -aimCamera.transform.position.z;

        Vector3 mouseWorld = aimCamera.ScreenToWorldPoint(mouse);
        Vector2 direction = mouseWorld - armPivot.position;
        aimDirection = direction.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (armPivot.parent.localScale.x < 0f)
            angle = 180f - angle;

        armPivot.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
