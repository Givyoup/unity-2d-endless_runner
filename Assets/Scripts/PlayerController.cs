using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    public float jumpForce = 12f;
    private bool isGrounded = true;

    public BoxCollider2D colliderRun;
    public BoxCollider2D colliderSlide;

    private bool isSliding = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (GameManager.Instance.isGameOver) {
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameManager.Instance.NewGame();
            }
        }

        // --- JUMP ---
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isSliding)
        {
            rb.linearVelocity = Vector2.up * jumpForce;
            isGrounded = false;
            animator.SetBool("isJump", true);

            // 🔊 Mainkan sound jump
            SoundManager.Instance.Play("jump");
        }

        // --- SLIDE ---
        if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && isGrounded)
        {
            if (!isSliding)
            {
                isSliding = true;
                colliderRun.enabled = false;
                colliderSlide.enabled = true;
                animator.SetBool("isSliding", true);

                // 🔊 Mainkan sound slide
                SoundManager.Instance.Play("slide");
            }
        }
        else if (isSliding && isGrounded)
        {
            isSliding = false;
            colliderRun.enabled = true;
            colliderSlide.enabled = false;
            animator.SetBool("isSliding", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("isGrounded", true);
            animator.SetBool("isJump", false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("isGrounded", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            GameManager.Instance.GameOver();
            animator.SetBool("isGameOver", true);

            // 🔊 Mainkan sound death
            SoundManager.Instance.Play("death");
        }
    }
}
