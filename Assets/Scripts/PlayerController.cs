using UnityEngine;

public sealed class PlayerController : MonoBehaviour
{
    [Header("Move")]
    public Rigidbody2D rb;
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 13f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Animation")]
    public Animator animator;
    public string moveBool = "IsMove";
    public string groundBool = "IsGround";
    public string jumpTrigger = "jump";

    [Header("Face")]
    public Camera aimCamera;

    public bool movementLocked;

    private float moveX;
    private bool isGrounded;
    private Vector3 startScale;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startScale = transform.localScale;
    }

    private void Update()
    {
        FaceMouse();

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (movementLocked)
        {
            moveX = 0f;
        }
        else
        {
            moveX = Input.GetAxisRaw("Horizontal");

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                animator.SetTrigger(jumpTrigger);
            }
        }

        animator.SetBool(moveBool, moveX != 0f);
        animator.SetBool(groundBool, isGrounded);
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);
    }

    public void SetMovementLocked(bool locked)
    {
        movementLocked = locked;
    }

    private void FaceMouse()
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = -aimCamera.transform.position.z;

        Vector3 mouseWorld = aimCamera.ScreenToWorldPoint(mouse);

        if (mouseWorld.x < transform.position.x)
            transform.localScale = new Vector3(-startScale.x, startScale.y, startScale.z);
        else
            transform.localScale = startScale;
    }
}
