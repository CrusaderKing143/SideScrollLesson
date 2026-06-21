using UnityEngine;

[DefaultExecutionOrder(100)]
public class OnLadder : MonoBehaviour
{
    [Header("Slide")]
    public bool reverseDirection;
    public float slideSpeed = 3f;
    public bool useMovePosition = true;

    [Header("Slope Jump")]
    public bool usePlayerJumpForce = true;
    public float slopeJumpSpeed = 9f;
    [Range(0f, 89f)] public float slopeJumpAngle = 45f;
    public bool jumpTowardDownSlope = true;
    public float jumpReleaseTime = 0.18f;

    [Header("Debug")]
    [SerializeField] private bool debugPlayerInside;
    [SerializeField] private float debugCurrentInput;
    [SerializeField] private Vector2 debugSlopeDirection;
    [SerializeField] private Vector2 debugTargetVelocity;
    [SerializeField] private Vector2 debugJumpVelocity;

    private PlayerController player;
    private Rigidbody2D playerRb;
    private float releaseUntil;
    private float releaseHorizontalVelocity;
    private bool clearAfterRelease;

    private void Update()
    {
        if (!playerRb)
            return;

        if (Input.GetButtonDown("Jump"))
            JumpOffSlope();
    }

    private void FixedUpdate()
    {
        debugPlayerInside = playerRb;
        debugTargetVelocity = Vector2.zero;

        if (!playerRb)
            return;

        if (Time.time < releaseUntil)
        {
            KeepJumpHorizontalVelocity();
            return;
        }

        if (clearAfterRelease)
        {
            ClearCurrentPlayer();
            return;
        }

        debugCurrentInput = Input.GetAxisRaw("Horizontal");

        Vector2 targetVelocity = GetDownSlopeDirection(GetSlopeDirection()) * slideSpeed;

        debugTargetVelocity = targetVelocity;
        playerRb.velocity = targetVelocity;

        if (useMovePosition)
            playerRb.MovePosition(playerRb.position + targetVelocity * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TrySetPlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TrySetPlayer(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ClearPlayer(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TrySetPlayer(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TrySetPlayer(collision.collider);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        ClearPlayer(collision.collider);
    }

    private void TrySetPlayer(Collider2D other)
    {
        PlayerController foundPlayer = other.GetComponentInParent<PlayerController>();

        if (!foundPlayer)
            return;

        player = foundPlayer;
        playerRb = foundPlayer.rb ? foundPlayer.rb : foundPlayer.GetComponent<Rigidbody2D>();
        clearAfterRelease = false;
    }

    private void ClearPlayer(Collider2D other)
    {
        PlayerController foundPlayer = other.GetComponentInParent<PlayerController>();

        if (!foundPlayer || foundPlayer != player)
            return;

        if (Time.time < releaseUntil)
        {
            clearAfterRelease = true;
            return;
        }

        ClearCurrentPlayer();
    }

    private void ClearCurrentPlayer()
    {
        player = null;
        playerRb = null;
        releaseHorizontalVelocity = 0f;
        clearAfterRelease = false;
        debugPlayerInside = false;
        debugCurrentInput = 0f;
        debugTargetVelocity = Vector2.zero;
        debugJumpVelocity = Vector2.zero;
    }

    private Vector2 GetSlopeDirection()
    {
        Vector2 slopeDirection = transform.right.normalized;

        if (reverseDirection)
            slopeDirection = -slopeDirection;

        debugSlopeDirection = slopeDirection;
        return slopeDirection;
    }

    private Vector2 GetDownSlopeDirection(Vector2 slopeDirection)
    {
        return slopeDirection.y <= 0f ? slopeDirection : -slopeDirection;
    }

    private void JumpOffSlope()
    {
        Vector2 jumpVelocity = GetSlopeJumpVelocity();

        releaseUntil = Time.time + jumpReleaseTime;
        releaseHorizontalVelocity = jumpVelocity.x;
        debugJumpVelocity = jumpVelocity;
        playerRb.velocity = jumpVelocity;
    }

    private void KeepJumpHorizontalVelocity()
    {
        playerRb.velocity = new Vector2(releaseHorizontalVelocity, playerRb.velocity.y);
        debugTargetVelocity = playerRb.velocity;
    }

    private Vector2 GetSlopeJumpVelocity()
    {
        Vector2 downSlopeDirection = GetDownSlopeDirection(GetSlopeDirection());
        float horizontalSign = Mathf.Abs(downSlopeDirection.x) > 0.01f ? Mathf.Sign(downSlopeDirection.x) : 1f;

        if (!jumpTowardDownSlope)
            horizontalSign = -horizontalSign;

        float angleRadians = slopeJumpAngle * Mathf.Deg2Rad;
        Vector2 jumpDirection = new Vector2(
            Mathf.Cos(angleRadians) * horizontalSign,
            Mathf.Sin(angleRadians)
        ).normalized;
        float jumpSpeed = usePlayerJumpForce && player ? player.jumpForce : slopeJumpSpeed;

        return jumpDirection * jumpSpeed;
    }
}
