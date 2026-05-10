using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class ArmAimController : MonoBehaviour
{
    [Header("References")]
    public Transform armPivot;
    public Transform playerRoot;
    public Camera aimCamera;

    [Header("Aim")]
    public float minAngle = -55f;
    public float maxAngle = 55f;
    public float angleOffset = 0f;

    private void Reset()
    {
        armPivot = transform;
        playerRoot = transform.root;
    }

    private void Awake()
    {
        if (armPivot == null)
            armPivot = transform;

        if (playerRoot == null)
            playerRoot = transform.root;
    }

    private void LateUpdate()
    {
        RefreshAim();
    }

    public void RefreshAim()
    {
        Camera cam = aimCamera != null ? aimCamera : Camera.main;
        if (cam == null || armPivot == null || playerRoot == null)
            return;

        Vector3 mouseScreen = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : Input.mousePosition;

        mouseScreen.z = cam.WorldToScreenPoint(armPivot.position).z;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);

        Vector2 pivotLocal = playerRoot.InverseTransformPoint(armPivot.position);
        Vector2 mouseLocal = playerRoot.InverseTransformPoint(mouseWorld);
        Vector2 direction = mouseLocal - pivotLocal;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + angleOffset;
        angle = Mathf.Clamp(angle, minAngle, maxAngle);

        armPivot.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
