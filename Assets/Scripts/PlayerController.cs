using UnityEngine;

[DisallowMultipleComponent]
public sealed class PlayerController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 10f;
    public float doubleJumpForce = 8f;

    [Header("Ground Check (Raycast)")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundRayLength = 0.25f;
    public float groundRayYOffset = 0.1f;

    [Header("Animator (Optional)")]
    public Animator playeranim;
    public string runBool = "run";
    public string groundedBool = "isGrounded";
    public string jumpTrigger = "jump";

    private Rigidbody2D rb;
    private float moveX;
    private bool isGroundedBool;
    private bool canDoubleJump;

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
        isGroundedBool = IsGrounded();

        if (isGroundedBool)
        {
            canDoubleJump = true;

            if (Input.GetButtonDown("Jump"))
            {
                Jump(jumpForce);
            }
        }
        else
        {
            if (canDoubleJump && Input.GetButtonDown("Jump"))
            {
                Jump(doubleJumpForce);
                canDoubleJump = false;
            }
        }

        SetAnimations();

        if (moveX != 0f)
        {
            FlipSprite(moveX);
        }
    }

    private void FixedUpdate()
    {
        moveX = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
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

    private void FlipSprite(float direction)
    {
        Vector3 s = transform.localScale;
        if (direction > 0f) s.x = Mathf.Abs(s.x);
        else if (direction < 0f) s.x = -Mathf.Abs(s.x);
        transform.localScale = s;
    }
}

