using UnityEngine;
using UnityEngine.InputSystem; // 【输入】新 Input System：用 Keyboard.current 读键，绕过 Both 模式下旧 Horizontal 轴延迟

[DisallowMultipleComponent]
public sealed class PlayerController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 10f;

    [Header("Ground Check (Raycast)")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundRayLength = 0.25f;
    public float groundRayYOffset = 0.1f;

    [Header("鼠标朝向")]
    [Tooltip("为 true：每帧按鼠标相对角色水平中线翻面（过中线转身）；为 false：仅按左右移动键翻面")]
    public bool faceTowardMouse = true;

    [Tooltip("与 ShootController 一致即可；不填则用 Camera.main")]
    public Camera aimCamera;

    [Tooltip("鼠标世界 X 与角色 X 接近时不翻面，减轻中线抖动")]
    public float mouseFacingEpsilon = 0.02f;

    [Header("Animator (Optional)")]
    public Animator playeranim;
    public string runBool = "run";
    public string groundedBool = "isGrounded";
    public string jumpTrigger = "jump";

    private Rigidbody2D rb;
    private float moveX;
    private bool isGroundedBool;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        playeranim = GetComponentInChildren<Animator>();
        groundCheck = transform;
    }

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (playeranim == null) playeranim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // 【输入采样】必须在 Update：松手当帧即可得到 moveX=0；若在 FixedUpdate 读轴仍可能与动画/翻面差一拍
        moveX = ReadHorizontalMove();

        isGroundedBool = IsGrounded();

        if (isGroundedBool && Input.GetButtonDown("Jump"))
        {
            Jump(jumpForce);
        }

        SetAnimations();

        ApplyFacingFromMouseOrMove();
    }

    private void FixedUpdate()
    {
        // 【物理】只用上一帧 Update 已算好的 moveX，此处不再读 Input，避免与输入系统混用时重复/滞后
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
    }

    /// <summary>
    /// 直接用键盘按键读取，避免旧 Input Manager / Input System 混用时 Horizontal 轴仍有衰减或延迟。
    /// </summary>
    private static float ReadHorizontalMove()
    {
        Keyboard kb = Keyboard.current;
        if (kb != null)
        {
            float x = 0f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) x -= 1f; // 【松手即停】按键布尔，无 Gravity 衰减
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x += 1f; // 同上（右移）
            return Mathf.Clamp(x, -1f, 1f); // 松开任意一边立刻反映为 0 / ±1，无平滑
        }

        // 【兜底】无键盘设备实例时退回旧接口（编辑器外少见）
        return Input.GetAxisRaw("Horizontal");
    }

    private void Jump(float force)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        if (playeranim != null && !string.IsNullOrEmpty(jumpTrigger))
        {
            playeranim.SetTrigger(jumpTrigger);
        }
    }

    private bool IsGrounded()
    {
        if (groundCheck == null) return false;

        Vector2 rayOrigin = new Vector2(groundCheck.position.x, groundCheck.position.y - groundRayYOffset);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundRayLength, groundLayer);
        return hit.collider != null;
    }

    private void SetAnimations()
    {
        if (playeranim == null) return;

        bool running = moveX != 0f && isGroundedBool;
        Debug.Log("running: " + running);
        if (!string.IsNullOrEmpty(runBool)) playeranim.SetBool(runBool, running);
        if (!string.IsNullOrEmpty(groundedBool)) playeranim.SetBool(groundedBool, isGroundedBool);
    }

    private void ApplyFacingFromMouseOrMove()
    {
        if (faceTowardMouse && TryGetMouseWorldPoint(out Vector2 mouseWorld))
        {
            float px = transform.position.x;
            if (mouseWorld.x > px + mouseFacingEpsilon)
                FlipSprite(1f);
            else if (mouseWorld.x < px - mouseFacingEpsilon)
                FlipSprite(-1f);
            return;
        }

        if (moveX != 0f)
            FlipSprite(moveX);
    }

    private bool TryGetMouseWorldPoint(out Vector2 world)
    {
        world = default;
        Camera cam = aimCamera != null ? aimCamera : Camera.main;
        if (cam == null)
            return false;

        Vector3 screen = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : (Vector3)Input.mousePosition;
        screen.z = cam.WorldToScreenPoint(transform.position).z;
        world = cam.ScreenToWorldPoint(screen);
        return true;
    }

    private void FlipSprite(float direction)
    {
        Vector3 s = transform.localScale;
        if (direction > 0f) s.x = Mathf.Abs(s.x);
        else if (direction < 0f) s.x = -Mathf.Abs(s.x);
        transform.localScale = s;
    }
}


