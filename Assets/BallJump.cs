using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BallJump : MonoBehaviour
{
    public float horizontalForce = 15f;
    public float bounceForce = 20f;
    public float jumpForce = 10f;  // Jump force
    public Transform groundCheck;  // Position to check if the ball is grounded
    public LayerMask groundLayer;  // Layer mask for ground detection

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // קישור ה-Rigidbody של הכדור
    }

    void Update()
    {
        // Check if the ball is grounded by overlapping the groundCheck position
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        // If the ball is grounded and the player presses the space key, apply the jump force
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            // Apply jump force
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            Vector3 ballPosition = transform.position;
            Vector3 platformPosition = collision.transform.position;
            float platformWidth = collision.collider.bounds.size.x;

            float hitPoint = (ballPosition.x - platformPosition.x) / (platformWidth / 2);
            float xForce = hitPoint * horizontalForce;

            // הגדרת מהירות חדשה
            rb.linearVelocity = new Vector2(xForce, bounceForce);
            Debug.Log($"HitPoint: {hitPoint}, Velocity: {rb.linearVelocity}");
        }
    }

}
